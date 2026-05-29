using UnityEditor;
using UnityEngine;

public static class WeaponPartCreater
{
    [MenuItem("Weapon Parts/New Weapon Part")]
    public static void CreateNewWeaponPart()
    {
        GameObject newPart = new GameObject("New Weapon Part");
        newPart.AddComponent<WeaponPart>();
        newPart.AddComponent<MeshFilter>();
        newPart.AddComponent<MeshRenderer>();

        Undo.RegisterCreatedObjectUndo(newPart, "Created New Weapon Part");
        Selection.activeGameObject = newPart;
    }
}