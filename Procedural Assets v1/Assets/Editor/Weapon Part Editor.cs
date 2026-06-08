using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeaponPart))]
public class WeaponPartEditor : Editor
{
    void OnEnable()
    {
        WeaponPart part = (WeaponPart)target;
        if (part.partMaterial == null)
            part.partMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/DefaultWeaponPart.mat");
    }
    
    public override void OnInspectorGUI()
    {
        WeaponPart part = (WeaponPart)target;
        serializedObject.Update();
        bool hasFrontAttach = part.stats.WeaponPartType != WeaponPartType.Blade 
                              && part.stats.WeaponPartType != WeaponPartType.Pendant;
        
        Rect rect = EditorGUILayout.GetControlRect(false, 10);
        EditorGUI.DrawRect(rect, WeaponPartUtils.GetRarityColor(part.stats.WeaponPartRarity));
        
        SerializedProperty statsProperty = serializedObject.FindProperty("stats");
        
        SerializedProperty nameProp = statsProperty.FindPropertyRelative("weaponPartName");
        EditorGUILayout.PropertyField(nameProp);
        
        SerializedProperty weaponTypeProp = statsProperty.FindPropertyRelative("weaponType");
        if (part.stats.WeaponPartType == WeaponPartType.Pendant)
        {
            part.stats.weaponType = WeaponType.None;
            EditorUtility.SetDirty(part);
        }
        else
        {
            EditorGUILayout.PropertyField(weaponTypeProp);
        }
        
        SerializedProperty weaponPartTypeProp = statsProperty.FindPropertyRelative("WeaponPartType");
        EditorGUILayout.PropertyField(weaponPartTypeProp);
        
        SerializedProperty rarityProp = statsProperty.FindPropertyRelative("WeaponPartRarity");
        EditorGUILayout.PropertyField(rarityProp);
        
        SerializedProperty sourceModelProp = serializedObject.FindProperty("sourceFBX");
        EditorGUILayout.PropertyField(sourceModelProp);
        
        if (part.stats.WeaponPartType == WeaponPartType.Handle)
        {
            EditorGUI.BeginDisabledGroup(true);
            SerializedProperty attachBackProp = serializedObject.FindProperty("attachBack");
            EditorGUILayout.PropertyField(attachBackProp);
            EditorGUI.EndDisabledGroup();
        }

        if (hasFrontAttach)
        {
            EditorGUI.BeginDisabledGroup(true);
            SerializedProperty attachFrontProp = serializedObject.FindProperty("attachFront");
            EditorGUILayout.PropertyField(attachFrontProp);
            EditorGUI.EndDisabledGroup();
        }
        
        SerializedProperty materialProp = serializedObject.FindProperty("partMaterial");
        EditorGUILayout.PropertyField(materialProp);

        if (part.sourceFBX != null && GUILayout.Button("Apply Model & Material"))
        {
            MeshFilter sourceMF = part.sourceFBX.GetComponentInChildren<MeshFilter>();
            if (sourceMF != null)
                part.GetComponent<MeshFilter>().sharedMesh = sourceMF.sharedMesh;
            
            if (part.stats.WeaponPartType == WeaponPartType.Handle)
            {
                Transform ab = FindAttachPoint(part.sourceFBX.transform, "aB");
                if (ab != null) part.attachBack = ab;
                if (ab != null) part.attachBackLocalPos = part.sourceFBX.transform.InverseTransformPoint(ab.position);
            }

            if (hasFrontAttach)
            {
                Transform a = FindAttachPoint(part.sourceFBX.transform, "a");
                if (a != null) part.attachFront = a;
                if (a != null) part.attachFrontLocalPos = part.sourceFBX.transform.InverseTransformPoint(a.position);
            }
            
            if (part.partMaterial != null)
                part.GetComponent<MeshRenderer>().sharedMaterial = part.partMaterial;

            EditorUtility.SetDirty(part);
        }
        
        if (part.stats.WeaponPartType != WeaponPartType.Handle && part.attachBack != null)
        {
            part.attachBack = null;
            EditorUtility.SetDirty(part);
        }
        
        if (!hasFrontAttach && part.attachFront != null)
        {
            part.attachFront = null;
            EditorUtility.SetDirty(part);
        }
        
        GUILayout.Space(8);
        EditorGUILayout.LabelField("Save", EditorStyles.boldLabel);

        if (GUILayout.Button("Save as Prefab"))
        {
            if (part.stats.WeaponPartType == WeaponPartType.Pendant)
            {
                EnsureFolder("Assets/WeaponParts", "Pendant");
            }
            else
            {
                EnsureFolder("Assets", "WeaponParts");
                EnsureFolder("Assets/WeaponParts", $"{part.stats.weaponType}");
                EnsureFolder($"Assets/WeaponParts/{part.stats.weaponType}", $"{part.stats.WeaponPartType}");
            }
            PrefabUtility.SaveAsPrefabAssetAndConnect(part.gameObject, GetSavePath(), InteractionMode.UserAction, out bool success);
    
            if (success) Debug.Log($"Saved prefab to {GetSavePath()}");
            else Debug.LogError($"Failed to save prefab to {GetSavePath()}");

            AssetDatabase.Refresh();
        }

        EditorGUILayout.Space(); 
        EditorGUILayout.Space();
        rect = EditorGUILayout.GetControlRect(false, 10);
        EditorGUI.DrawRect(rect, WeaponPartUtils.GetRarityColor(part.stats.WeaponPartRarity));

        serializedObject.ApplyModifiedProperties();
    }

    string GetSavePath()
    {
        WeaponPart part = (WeaponPart)target;
    
        if (part.stats.WeaponPartType == WeaponPartType.Pendant)
            return $"Assets/WeaponParts/Pendant/{part.stats.weaponPartName}.prefab";
    
        return $"Assets/WeaponParts/{part.stats.weaponType}/{part.stats.WeaponPartType}/{part.stats.weaponPartName}.prefab";
    }
    
    void EnsureFolder(string parentPath, string folderName)
    {
        string fullPath = $"{parentPath}/{folderName}";
        
        if (!AssetDatabase.IsValidFolder(fullPath))
            AssetDatabase.CreateFolder(parentPath, folderName);
    }
    
    Transform FindAttachPoint(Transform root, string baseName)
    {
        foreach (Transform child in root.GetComponentsInChildren<Transform>())
            if (child.name == baseName || child.name.StartsWith(baseName + "."))
                return child;
        
        return null;
    }
}