using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class WeaponPartOverview : EditorWindow
{
    [MenuItem("Weapon Parts/Open Parts Overview")]
    public static void Open() => GetWindow<WeaponPartOverview>("Weapon Part Overview");
    
    List<WeaponPart> loadedParts = new List<WeaponPart>();
    WeaponPartRegistry registry;

    void LoadParts()
    {
        loadedParts.Clear();
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/WeaponParts" });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            WeaponPart part = prefab.GetComponent<WeaponPart>();
            if (part != null) loadedParts.Add(part);
        }
    }
    
    void OnFocus() => LoadParts();

    void OnGUI()
    {
        EditorGUILayout.Space();
        
        registry = (WeaponPartRegistry)EditorGUILayout.ObjectField("Part Registry", registry, typeof(WeaponPartRegistry), false);
        
        if (registry != null && GUILayout.Button("Populate Registry"))
        {
            registry.parts = new List<WeaponPart>(loadedParts);
            EditorUtility.SetDirty(registry);
            AssetDatabase.SaveAssets();
            Debug.Log($"Registry populated with {registry.parts.Count} parts.");
        }
        
        EditorGUILayout.Space();
        
        int columnWidth = 120;
        int rowHeight = 60;
        int columns = Mathf.Max(1, Mathf.FloorToInt(EditorGUIUtility.currentViewWidth / columnWidth));

        var pendants = loadedParts.Where(p => p.stats.WeaponPartType == WeaponPartType.Pendant);
        var nonPendants = loadedParts.Where(p => p.stats.WeaponPartType != WeaponPartType.Pendant);

        var byWeaponType = nonPendants
            .GroupBy(p => p.stats.weaponType)
            .OrderBy(g => (int)g.Key);

        foreach (var group in byWeaponType)
        {
            GUILayout.Label(group.Key.ToString(), EditorStyles.boldLabel);

            var byPartType = group
                .GroupBy(p => p.stats.WeaponPartType)
                .OrderBy(g => (int)g.Key);

            foreach (var partGroup in byPartType)
            {
                GUILayout.Label(partGroup.Key.ToString());

                int current = 0;
                GUILayout.BeginHorizontal();

                foreach (var part in partGroup.OrderByDescending(p => p.stats.WeaponPartRarity))
                {
                    GUI.backgroundColor = WeaponPartUtils.GetRarityColor(part.stats.WeaponPartRarity);
                    GUIStyle style = new GUIStyle(GUI.skin.button);
                    style.fontSize = Mathf.Clamp(36 - part.stats.weaponPartName.Length, 9, 14);
                    style.wordWrap = true;

                    if (GUILayout.Button($"{part.stats.weaponPartName}", style, GUILayout.Width(columnWidth), GUILayout.Height(rowHeight)))
                    {
                        string path = AssetDatabase.GetAssetPath(part.gameObject);
                        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                        EditorGUIUtility.PingObject(prefab);
                        Selection.activeGameObject = prefab;
                    }
                    GUI.backgroundColor = Color.white;

                    current++;
                    if (current % columns == 0)
                    {
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                    }
                }

                GUILayout.EndHorizontal();
                GUILayout.Space(6);
            }
            
            GUILayout.Space(14);
        }

        if (pendants.Any())
        {
            GUILayout.Space(14);
            GUILayout.Label("Pendants", EditorStyles.boldLabel);

            int current = 0;
            GUILayout.BeginHorizontal();

            foreach (var part in pendants.OrderByDescending(p => p.stats.WeaponPartRarity))
            {
                GUI.backgroundColor = WeaponPartUtils.GetRarityColor(part.stats.WeaponPartRarity);
                GUIStyle style = new GUIStyle(GUI.skin.button);
                style.fontSize = Mathf.Clamp(36 - part.stats.weaponPartName.Length, 9, 14);
                style.wordWrap = true;

                if (GUILayout.Button(part.stats.weaponPartName, style, GUILayout.Width(columnWidth), GUILayout.Height(rowHeight)))
                {
                    string path = AssetDatabase.GetAssetPath(part.gameObject);
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    EditorGUIUtility.PingObject(prefab);
                    Selection.activeGameObject = prefab;
                }
                GUI.backgroundColor = Color.white;

                current++;
                if (current % columns == 0)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
            }

            GUILayout.EndHorizontal();
        }
    }
}