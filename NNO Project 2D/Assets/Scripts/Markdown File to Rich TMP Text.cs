using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class MarkdownFileToRichTMPText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmp;
    [SerializeField] private TextAsset markdownFile;
    [Header("Font Sizes")] 
    [SerializeField] private int titleFS = 102;
    [SerializeField] private int subtitleFS = 90;
    [SerializeField] private int headingFS = 78;
    [SerializeField] private int subheadingFS = 72;
    [SerializeField] private int sectionFS = 60;
    [SerializeField] private int subsectionFS = 54;
    [SerializeField] private int bodyFS = 48;
    
    private string[] lines;
    
    public void Start() => GenerateText();

    public void GenerateText()
    {
        StringBuilder builder = new StringBuilder();

        string raw = markdownFile.text;

        raw = raw.Replace("\r\n", "\n").Replace("\r", "\n");
        raw = raw.Trim();
        raw = Regex.Replace(raw, @"\n\s*\n+", "\n\n");

        lines = raw.Split('\n');

        bool lastWasText = false;

        foreach (string line in lines)
        {
            string trimmed = line.Trim();
            bool isHeader = trimmed.StartsWith("#");

            if (string.IsNullOrWhiteSpace(trimmed))
            {
                if (!lastWasText)
                    builder.AppendLine();

                lastWasText = false;
                continue;
            }

            AppendFormattedLine(builder, trimmed);
            lastWasText = isHeader;
        }

        tmp.maxVisibleCharacters = builder.Length + 100;    //100 buffer
        tmp.text = builder.ToString();
    }
    
    private void AppendFormattedLine(StringBuilder builder, string line)
    {
        int fontSize;

        if (line.StartsWith("# ")) { line = line.Substring(2); fontSize = titleFS; }
        else if (line.StartsWith("## ")) { line = line.Substring(3); fontSize = subtitleFS; }
        else if (line.StartsWith("### ")) { line = line.Substring(4); fontSize = headingFS; }
        else if (line.StartsWith("#### ")) { line = line.Substring(5); fontSize = subheadingFS; }
        else if (line.StartsWith("##### ")) { line = line.Substring(6); fontSize = sectionFS; }
        else if (line.StartsWith("###### ")) { line = line.Substring(7); fontSize = subsectionFS; }
        else fontSize = bodyFS;

        line = ConvertInlineMarkdown(line);

        builder.AppendLine($"<size={fontSize}>{line}</size>");
    }
    
    string ConvertInlineMarkdown(string input)
    {
        input = Regex.Replace(input, @"\*\*(.*?)\*\*", "<b>$1</b>");
        input = Regex.Replace(input, @"\*(.*?)\*", "<i>$1</i>");
        //input = Regex.Replace(input, @"_(.*?)_", "<u>$1</u>");
        input = Regex.Replace(input, @"_(.*?)_", "<i>$1</i>");//Temporary underline to italics
        input = Regex.Replace(input, @"---", "");

        return input;
    }
}