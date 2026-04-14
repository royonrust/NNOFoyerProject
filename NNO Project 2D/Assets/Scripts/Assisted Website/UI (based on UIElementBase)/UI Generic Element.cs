using System;
using UnityEngine;

public class UIGenericElement : UIElementBase
{
    public override UIElementData GenerateData()
    {
        var d = new UIGenericElementData();
        
        PopulateIDAndRectData(d);
        return d;
    }

    public override void ApplyCustomData(UIElementData baseData)
    {
    }
}

[Serializable]
public class UIGenericElementData : UIElementData
{
    
}