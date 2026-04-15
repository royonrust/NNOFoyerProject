using System;
using System.Collections.Generic;
using UnityEngine;

public class UIGenericElement : UIElementBase
{
    public override UIElementData GenerateData()
    {
        return new UIElementData
        {
            prefabID = prefabID,
            anchoredPosition = ((RectTransform)transform).anchoredPosition,
            children = new List<UIElementData>()
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