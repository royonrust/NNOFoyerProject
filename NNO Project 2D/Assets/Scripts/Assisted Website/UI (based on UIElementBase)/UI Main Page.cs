using System;
using System.Collections.Generic;
using UnityEngine;

public class UIMainPage : UIElementBase
{
    public PageType pageType = PageType.home;
    [HideInInspector] public bool isHidden;
    
    public override UIElementData GenerateData()
    {
        return new UIMainPageData
        {
            prefabID = prefabID,
            anchoredPosition = ((RectTransform)transform).anchoredPosition,
            type = pageType,
            hiddenPage = isHidden
        };
    }

    public override void ApplyData(UIElementData baseData)
    {
        var d = (UIMainPageData)baseData;

        pageType = d.type;
        isHidden = d.hiddenPage;
        
        if (isHidden && gameObject.activeSelf) 
            gameObject.SetActive(false);
    }
}

[Serializable]
public class UIMainPageData : UIElementData
{
    public PageType type;
    public bool hiddenPage;
}