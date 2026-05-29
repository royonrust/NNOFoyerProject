using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class WeaponPartOverview : EditorWindow
{
    [MenuItem("Weapon Parts/Open Parts Overview")]
    public static void Open() => GetWindow<WeaponPartOverview>("Weapon Part Overview");
    
    List<WeaponPart> loadedParts = new List<WeaponPart>();

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

    void OnGUI()
    {
        if (GUILayout.Button("Refresh")) LoadParts();

        int columnWidth = 120;
        int rowHeight = 60;
        int columns = Mathf.Max(1, Mathf.FloorToInt(EditorGUIUtility.currentViewWidth / columnWidth));

        var byWeaponType = loadedParts
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
                    style.fontSize = Mathf.Clamp(36 - part.stats.partName.Length, 9, 14);
                    style.wordWrap = true;

                    if (GUILayout.Button($"{part.stats.partName}\n{part.stats.WeaponPartType}", style, GUILayout.Width(columnWidth), GUILayout.Height(rowHeight)))
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
                }                          // ← inner foreach ends here

                GUILayout.EndHorizontal(); // ← outside inner foreach
                GUILayout.Space(6);        // ← outside inner foreach
            }
            
            GUILayout.Space(14);
        }
    }
}