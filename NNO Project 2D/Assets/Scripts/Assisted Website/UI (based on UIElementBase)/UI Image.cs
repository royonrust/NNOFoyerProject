using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteAlways]
public class UIImage : DraggableUIElement
{
    [SerializeField] private Image image;

    public override UIElementData GenerateData()
    {
        return new UIImageData
        {
            prefabID = prefabID,
            anchoredPosition = ((RectTransform)transform).anchoredPosition,
            spriteName = image.sprite != null ? image.sprite.name : string.Empty
        };
    }

    public override void ApplyCustomData(UIElementData baseData)
    {
        var d = (UIImageData)baseData;
        if (!string.IsNullOrEmpty(d.spriteName))
            image.sprite = Resources.Load<Sprite>("Sprites/" + d.spriteName);
    }

    public override void OnBeginDrag(PointerEventData eventData) { }
    public override void OnEndDrag(PointerEventData eventData) => RefreshPageDimensions();
    public override void OnPointerClick(PointerEventData eventData) { }
}

[Serializable]
public class UIImageData : UIElementData
{
    public string spriteName;
}