using System;
using UnityEngine;

public class UIMainPage : UIElementBase
{
    public override UIElementData GenerateData()
    {
        return new UITextblockData
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
public class UIMainPageData : UIElementData
{
    
}