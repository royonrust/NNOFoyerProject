using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeaponPart))]
public class WeaponPartEditor : Editor
{
    public override void OnInspectorGUI()
    {
        WeaponPart part = (WeaponPart)target;
        serializedObject.Update();
        
        SerializedProperty statsProperty = serializedObject.FindProperty("stats");
        
        SerializedProperty nameProp = statsProperty.FindPropertyRelative("partName");
        EditorGUILayout.PropertyField(nameProp);
        
        SerializedProperty weaponTypeProp = statsProperty.FindPropertyRelative("weaponType");
        EditorGUILayout.PropertyField(weaponTypeProp);
        
        SerializedProperty weaponPartTypeProp = statsProperty.FindPropertyRelative("WeaponPartType");
        EditorGUILayout.PropertyField(weaponPartTypeProp);
        
        SerializedProperty rarityProp = statsProperty.FindPropertyRelative("WeaponPartRarity");
        EditorGUILayout.PropertyField(rarityProp);
        Rect rect = EditorGUILayout.GetControlRect(false, 10);
        EditorGUI.DrawRect(rect, WeaponPartUtils.GetRarityColor(part.stats.WeaponPartRarity));
        
        SerializedProperty attachBackProp = serializedObject.FindProperty("attachBack");
        EditorGUILayout.PropertyField(attachBackProp);
        if (part.attachBack == null)
        {
            if (GUILayout.Button("Create Back Point"))
            {
                GameObject APB = new GameObject("AttachPoint_Back");
                Undo.RegisterCreatedObjectUndo(APB, "Create Back Attach Point");
                APB.transform.SetParent(part.transform);
                APB.transform.localPosition = Vector3.zero;
                part.attachBack = APB.transform;
                EditorUtility.SetDirty(part);
            }
        }

        if (part.stats.WeaponPartType != WeaponPartType.Pendant)
        {
            SerializedProperty attachFrontProp = serializedObject.FindProperty("attachFront");
            EditorGUILayout.PropertyField(attachFrontProp);
            if (part.attachFront == null)
            {
                if (GUILayout.Button("Create Front Point"))
                {
                    GameObject APF = new GameObject("AttachPoint_Front");
                    Undo.RegisterCreatedObjectUndo(APF, "Create Front Attach Point");
                    APF.transform.SetParent(part.transform);
                    APF.transform.localPosition = Vector3.zero;
                    part.attachFront = APF.transform;
                    EditorUtility.SetDirty(part);
                }
            }
        }
        
        if (GUILayout.Button("Save as Prefab"))
        {
            // Ensure each folder level exists before saving
            EnsureFolder("Assets", "WeaponParts");
            EnsureFolder("Assets/WeaponParts", $"{part.stats.weaponType}");
            EnsureFolder($"Assets/WeaponParts/{part.stats.weaponType}", $"{part.stats.WeaponPartType}");

            PrefabUtility.SaveAsPrefabAssetAndConnect(part.gameObject, GetSavePath(), InteractionMode.UserAction, out bool success);
    
            if (success) Debug.Log($"Saved prefab to {GetSavePath()}");
            else Debug.LogError($"Failed to save prefab to {GetSavePath()}");

            AssetDatabase.Refresh();
        }

        serializedObject.ApplyModifiedProperties();
    }
    
    void OnSceneGUI()
    {
        WeaponPart part = (WeaponPart)target;

        if (part.attachBack != null)
        {
            Handles.color = Color.blue;
            Handles.CubeHandleCap(0, part.attachBack.position, part.attachBack.rotation, .1f, EventType.Repaint);
            
            EditorGUI.BeginChangeCheck();
            Vector3 newPos = Handles.PositionHandle(part.attachBack.position, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(part.attachBack, "Move Back Attach Point");
                part.attachBack.position = newPos;
            }
        }
        
        if (part.attachFront != null)
        {
            Handles.color = Color.red;
            Handles.CubeHandleCap(0, part.attachFront.position, part.attachFront.rotation, .1f, EventType.Repaint);
            
            EditorGUI.BeginChangeCheck();
            Vector3 newPos = Handles.PositionHandle(part.attachFront.position, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(part.attachFront, "Move Front Attach Point");
                part.attachFront.position = newPos;
            }
        }
    }

    string GetSavePath()
    {
        WeaponPart part = (WeaponPart)target;
        return $"Assets/WeaponParts/{part.stats.weaponType}/{part.stats.WeaponPartType}/{part.stats.partName}.prefab";
    }
    
    void EnsureFolder(string parentPath, string folderName)
    {
        string fullPath = $"{parentPath}/{folderName}";
        if (!AssetDatabase.IsValidFolder(fullPath))
            AssetDatabase.CreateFolder(parentPath, folderName);
    }
}