using System.Globalization;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_InputField))]
public class InputFieldFormatter : MonoBehaviour
{
    private TMP_InputField inputField;
    private TMP_Text text;

    [SerializeField] private TMP_InputField fontInputField;

    [SerializeField] private RectTransform buttons;

    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
        text = inputField.textComponent;
    }
    
    public void ToggleBold() => text.fontStyle = ToggleStyle(text.fontStyle, FontStyles.Bold);
    public void ToggleItalics() => text.fontStyle = ToggleStyle(text.fontStyle, FontStyles.Italic);
    public void ToggleUnderline() => text.fontStyle = ToggleStyle(text.fontStyle, FontStyles.Underline);
    public void ToggleStrikethrough() => text.fontStyle = ToggleStyle(text.fontStyle, FontStyles.Strikethrough);

    public void IncreaseFontSize() => ApplyFontSize(text.fontSizeMax + 6);
    public void DecreaseFontSize() => ApplyFontSize(text.fontSizeMax - 6);
    public void SetFontSize() => ApplyFontSize(int.Parse(fontInputField.text));

    public void ApplyFontSize(float size)
    {
        if (text == null) 
            Awake();
        
        size = ClampFontSize(size);
        text.enableAutoSizing = true;
        text.fontSizeMin = 6;
        text.fontSizeMax = size;
        fontInputField.text = size.ToString(CultureInfo.CurrentCulture);
    }

    public void ApplyFontStyle(FontStyles style) => text.fontStyle = style;
    public void ApplyAlignment(TextAlignmentOptions alignment) => text.alignment = alignment;

    public void AlignLeft() => SetHorizontalAlignment(HorizontalAlignmentOptions.Left);
    public void AlignCenter() => SetHorizontalAlignment(HorizontalAlignmentOptions.Center);
    public void AlignRight() => SetHorizontalAlignment(HorizontalAlignmentOptions.Right);

    public void AlignTop() => SetVerticalAlignment(VerticalAlignmentOptions.Top);
    public void AlignMiddle() => SetVerticalAlignment(VerticalAlignmentOptions.Middle);
    public void AlignBottom() => SetVerticalAlignment(VerticalAlignmentOptions.Bottom);

    private void SetHorizontalAlignment(HorizontalAlignmentOptions horizontal) => text.horizontalAlignment = horizontal;
    private void SetVerticalAlignment(VerticalAlignmentOptions vertical) => text.verticalAlignment = vertical;

    public void DoneEditing()
    {
        buttons.SetParent(transform, true);
        GetComponentInParent<UITextblock>()?.SetTextFromInputField();
    }
    
    private void OnEnable() => buttons.SetParent(transform.root, true);
    private float ClampFontSize(float size) => Mathf.Clamp(size, 6, 144);

    private FontStyles ToggleStyle(FontStyles current, FontStyles flag)
    {
        bool isActive = (current & flag) != 0;
        return isActive ? current & ~flag : current | flag;
    }
}