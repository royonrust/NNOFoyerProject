using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIImage : DraggableUIElement
{
    [SerializeField] private Image image;
    
    public override UIElementData GenerateData()
    {
        return new UIImageData
        {
            prefabID = prefabID,
            anchoredPosition = ((RectTransform)transform).anchoredPosition,
            sprite = image.sprite
        };
    }

    public override void ApplyData(UIElementData baseData)
    {
        var d = (UIImageData)baseData;

        //image.sprite = d.sprite;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        
    }
}

[Serializable]
public class UIImageData : UIElementData
{
    [Newtonsoft.Json.JsonIgnore] public Sprite sprite;
}