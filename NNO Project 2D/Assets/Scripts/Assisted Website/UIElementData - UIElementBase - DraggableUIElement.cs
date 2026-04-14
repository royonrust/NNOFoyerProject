using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public abstract class UIElementData
{
    public string prefabID;
    public string spawnRootIdentifier;
    public List<UIElementData> children = new();
    
    public Vector2 anchoredPosition;
    public Vector2 sizeDelta;
    public Vector2 anchorMin;
    public Vector2 anchorMax;
    public Vector2 pivot;
}

[Serializable]
public struct NamedSpawnRoot
{
    public string identifier;
    public Transform root;
}

public abstract class UIElementBase : MonoBehaviour
{
    [SerializeField] private string id;
    public string prefabID => id;
    [NonSerialized] public UIElementData data;
    
    [SerializeField] private Transform contentRoot;
    public List<NamedSpawnRoot> namedSpawnRoots;

    public Transform GetSpawnRoot(string identifier = null)
    {
        if (!string.IsNullOrEmpty(identifier) && namedSpawnRoots != null)
        {
            var match = namedSpawnRoots.Find(r => r.identifier == identifier);
            if (match.root != null) return match.root;
        }
        return contentRoot != null ? contentRoot : transform;
    }
    
    public IEnumerable<Transform> GetAllSpawnRoots()
    {
        if (namedSpawnRoots != null && namedSpawnRoots.Count > 0)
            foreach (var namedRoot in namedSpawnRoots)
            foreach (Transform child in namedRoot.root)
                yield return child;
        else
            foreach (Transform child in GetSpawnRoot())
                yield return child;
    }
    
    public abstract UIElementData GenerateData();
    
    public void ApplyGeneralData(UIElementData importedData)
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.anchorMin = importedData.anchorMin;
        rect.anchorMax = importedData.anchorMax;
        rect.pivot = importedData.pivot;
        rect.anchoredPosition = importedData.anchoredPosition;
        rect.sizeDelta = importedData.sizeDelta;
        RefreshPageDimensions();
        ApplyCustomData(importedData);
    }

    protected void PopulateIDAndRectData(UIElementData d)
    {
        d.prefabID = prefabID;
        
        RectTransform rect = GetComponent<RectTransform>();
        d.anchoredPosition = rect.anchoredPosition;
        d.sizeDelta = rect.sizeDelta;
        d.anchorMin = rect.anchorMin;
        d.anchorMax = rect.anchorMax;
        d.pivot = rect.pivot;
    }
    
    public abstract void ApplyCustomData(UIElementData baseData);
    
    public void RefreshPageDimensions()
    {
        UIMainPage page = GetComponent<UIMainPage>() ?? GetComponentInParent<UIMainPage>();
    
        if (page == null)
            return;
    
        page.AdaptHeight();
    }
}

public abstract class DraggableUIElement : UIElementBase, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [HideInInspector] public bool canEdit;
    [HideInInspector] public bool isDraggable;
    [HideInInspector] public Transform parentAfterDrag;
    
    public abstract void OnBeginDrag(PointerEventData eventData);
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!canEdit || !isDraggable) return;
        transform.position = eventData.position;
    }
    
    public abstract void OnEndDrag(PointerEventData eventData);
    public abstract void OnPointerClick(PointerEventData eventData);
}