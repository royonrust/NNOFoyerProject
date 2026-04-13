using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
    [NonSerialized] public UIElementData data;
    
    [SerializeField] private Transform contentRoot;
    public Transform GetSpawnRoot()
    {
        return contentRoot != null ? contentRoot : transform;
    }

    public UIElementData SetupGenerateData()
    {
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

public abstract class DraggableUIElement : UIElementBase, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [HideInInspector] public bool canEdit;
    [HideInInspector] public Transform parentAfterDrag;
    
    public abstract void OnBeginDrag(PointerEventData eventData);
    public void OnDrag(PointerEventData eventData)
    {
        if (!canEdit) return;
        transform.position = eventData.position;
    }
    public abstract void OnEndDrag(PointerEventData eventData);
    public abstract void OnPointerClick(PointerEventData eventData);
}