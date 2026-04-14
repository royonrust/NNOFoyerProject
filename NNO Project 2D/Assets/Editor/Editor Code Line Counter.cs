using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

public class CodeLineCounter : EditorWindow //ChatGPT generated tool
{
    private int totalLines;
    private int filledLines;
    private int totalFiles;

    private Vector2 scrollPos;
    private string targetFolderPath = "";

    private List<FileStats> fileStats = new List<FileStats>();

    private enum SortMode
    {
        None,
        Alphabetical,
        TotalLines,
        FilledLines
    }

    private SortMode currentSortMode = SortMode.Alphabetical;

    [MenuItem("Tools/Code Line Counter")]
    public static void ShowWindow()
    {
        GetWindow<CodeLineCounter>("Code Line Counter");
    }

    private void OnGUI()
    {
        GUILayout.Label("Unity C# Line Counter", EditorStyles.boldLabel);

        GUILayout.Space(5);
        DrawFolderSelection();

        GUILayout.Space(10);

        if (GUILayout.Button("Count Lines"))
        {
            CountLines();
        }

        GUILayout.Space(10);

        GUILayout.Label($"C# Files: {totalFiles}");
        GUILayout.Label($"Total Lines: {totalLines}");
        GUILayout.Label($"Non-empty Lines: {filledLines}");

        GUILayout.Space(10);
        DrawSortButtons();

        GUILayout.Space(5);
        GUILayout.Label("Per-file breakdown:", EditorStyles.boldLabel);

        scrollPos = GUILayout.BeginScrollView(scrollPos);
        foreach (var stat in fileStats)
        {
            GUILayout.Label(
                $"{stat.FileName} — Total: {stat.TotalLines}, Filled: {stat.FilledLines}"
            );
        }
        GUILayout.EndScrollView();
    }

    private void DrawFolderSelection()
    {
        GUILayout.Label("Target Folder:");

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.TextField(string.IsNullOrEmpty(targetFolderPath) ? "None selected" : targetFolderPath);

        if (GUILayout.Button("Use Selected", GUILayout.Width(100)))
        {
            SetFolderFromSelection();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox(
            "Select a folder in the Project window, then click 'Use Selected'.",
            MessageType.Info
        );
    }

    private void DrawSortButtons()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Sort A–Z"))
        {
            currentSortMode = SortMode.Alphabetical;
            SortFiles();
        }

        if (GUILayout.Button("Sort by Total Lines"))
        {
            currentSortMode = SortMode.TotalLines;
            SortFiles();
        }

        if (GUILayout.Button("Sort by Non-empty Lines"))
        {
            currentSortMode = SortMode.FilledLines;
            SortFiles();
        }

        EditorGUILayout.EndHorizontal();
    }

    private void SetFolderFromSelection()
    {
        Object obj = Selection.activeObject;

        if (obj == null)
            return;

        string assetPath = AssetDatabase.GetAssetPath(obj);

        if (!AssetDatabase.IsValidFolder(assetPath))
            return;

        targetFolderPath = assetPath;
    }

    private void CountLines()
    {
        totalLines = 0;
        filledLines = 0;
        totalFiles = 0;
        fileStats.Clear();

        if (string.IsNullOrEmpty(targetFolderPath))
            return;

        string fullPath = Path.Combine(
            Application.dataPath,
            targetFolderPath.Replace("Assets/", "")
        );

        if (!Directory.Exists(fullPath))
            return;

        string[] files = Directory.GetFiles(
            fullPath,
            "*.cs",
            SearchOption.AllDirectories
        );

        foreach (string file in files)
        {
            int fileTotalLines = 0;
            int fileFilledLines = 0;

            string[] lines = File.ReadAllLines(file);

            foreach (string line in lines)
            {
                fileTotalLines++;
                if (!string.IsNullOrWhiteSpace(line))
                    fileFilledLines++;
            }

            totalFiles++;
            totalLines += fileTotalLines;
            filledLines += fileFilledLines;

            fileStats.Add(new FileStats
            {
                FileName = Path.GetFileName(file),
                TotalLines = fileTotalLines,
                FilledLines = fileFilledLines
            });
        }

        SortFiles();
    }

    private void SortFiles()
    {
        switch (currentSortMode)
        {
            case SortMode.Alphabetical:
                fileStats = fileStats
                    .OrderBy(f => f.FileName)
                    .ToList();
                break;

            case SortMode.TotalLines:
                fileStats = fileStats
                    .OrderByDescending(f => f.TotalLines)
                    .ToList();
                break;

            case SortMode.FilledLines:
                fileStats = fileStats
                    .OrderByDescending(f => f.FilledLines)
                    .ToList();
                break;
        }
    }

    private class FileStats
    {
        public string FileName;
        public int TotalLines;
        public int FilledLines;
    }
}
