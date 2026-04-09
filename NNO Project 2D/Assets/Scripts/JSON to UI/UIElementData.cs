using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class UIElementData
{
    public string prefabID;
    public Vector2 anchoredPosition;
    public string parentIdentifier;
    public List<UIElementData> children = new();
}

public abstract class UIElementBase : MonoBehaviour
{
    [SerializeField] private string id;
    public string prefabID => id;
    [HideInInspector] public string parentName;
    [NonSerialized] public UIElementData data;

    public UIElementData SetupGenerateData()
    {
        if (transform.parent != null) parentName = transform.parent.name;
        else parentName = null;
        return GenerateData();
    }
    public abstract UIElementData GenerateData();
    
    public void SetupApplyData(UIElementData importedData)
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.anchoredPosition = importedData.anchoredPosition;
        ApplyData(importedData);
    }
    public abstract void ApplyData(UIElementData baseData);
}