using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UILayoutSnappingArea : UIElementBase, IDropHandler
{
    private RectTransform rect;
    
    private void Start()
    {
        rect = GetComponent<RectTransform>();
        SetGridCellSize();
    }
    
    public override UIElementData GenerateData()
    {
        return new UILayoutSnappingAreaData
        {
            prefabID = prefabID,
            anchoredPosition = ((RectTransform)transform).anchoredPosition,
            rectSize = rect.rect.size
        };
    }

    public override void ApplyData(UIElementData baseData)
    {
        var d = (UILayoutSnappingAreaData)baseData;
        
        rect = GetComponent<RectTransform>();
        rect.sizeDelta = d.rectSize;
        SetGridCellSize();
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount > 0) return;
        
        GameObject dropped = eventData.pointerDrag;
        if (dropped.TryGetComponent(out DraggableUIElement element)) 
            element.parentAfterDrag = transform;
    }

    public void SetGridCellSize()
    {
        GridLayoutGroup grid = GetComponent<GridLayoutGroup>();
        grid.cellSize = rect.rect.size;
    }
}

[Serializable]
public class UILayoutSnappingAreaData : UIElementData
{
    public Vector2 rectSize;
}