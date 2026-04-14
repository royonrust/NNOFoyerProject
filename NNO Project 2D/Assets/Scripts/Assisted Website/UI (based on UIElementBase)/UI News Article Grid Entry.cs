using System;
using TMPro;
using UnityEngine;

public class UINewsArticleGridEntry : UIElementBase
{
    [SerializeField] private TextMeshProUGUI titleTMP;
    [SerializeField] private TextMeshProUGUI descriptionTMP;
    
    public override UIElementData GenerateData()
    {
        return new UINewsArticleData
        {
            prefabID = prefabID,
            anchoredPosition = ((RectTransform)transform).anchoredPosition,
            // title = titleTMP.text,
            // description = descriptionTMP.text
        };
    }

    public override void ApplyCustomData(UIElementData baseData)
    {
        var d = (UINewsArticleData)baseData;

        // titleTMP.text = d.title;
        // descriptionTMP.text = d.description;
    }
}

[Serializable]
public class UINewsArticleData: UIElementData
{
    public string title;
    public string description;
    
}