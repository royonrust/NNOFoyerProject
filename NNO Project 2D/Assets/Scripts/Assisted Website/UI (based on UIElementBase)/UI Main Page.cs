using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIMainPage : UIElementBase
{
    public PageType pageType = PageType.home;
    [HideInInspector] public bool isHidden;
    
    public override UIElementData GenerateData()
    {
        var d = new UIMainPageData
        {
            type = pageType,
            hiddenPage = isHidden
        };
        
        PopulateIDAndRectData(d);
        return d;
    }

    public override void ApplyCustomData(UIElementData baseData)
    {
        var d = (UIMainPageData)baseData;

        pageType = d.type;
        isHidden = d.hiddenPage;
        
        if (isHidden && gameObject.activeSelf) 
            gameObject.SetActive(false);
    }

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame(); 
        AdaptHeight();
    }

    public void AdaptHeight()
    {
        float contentHeight = LayoutUtility.GetPreferredHeight((RectTransform)GetSpawnRoot());
        
        RectTransform rect = GetComponent<RectTransform>();
        RectTransform canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, Mathf.Max(canvasRect.rect.height, contentHeight));
    }
}

[Serializable]
public class UIMainPageData : UIElementData
{
    public PageType type;
    public bool hiddenPage;
}