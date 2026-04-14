using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteAlways]
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
        rect ??= GetComponent<RectTransform>();
        
        var d = new UILayoutSnappingAreaData
        {
            rectSize = rect.rect.size
        };
        PopulateIDAndRectData(d);
        return d;
    }

    public override void ApplyCustomData(UIElementData baseData)
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

    private void OnEnable() => Start();

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