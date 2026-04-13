using System;
using UnityEngine;

public class UIGenericElement : UIElementBase
{
    public override UIElementData GenerateData()
    {
        return new UIGenericElementData
        {
            prefabID = prefabID,
            anchoredPosition = ((RectTransform)transform).anchoredPosition
        };
    }

    public override void ApplyData(UIElementData baseData)
    {
    }
}

[Serializable]
public class UIGenericElementData : UIElementData
{
}