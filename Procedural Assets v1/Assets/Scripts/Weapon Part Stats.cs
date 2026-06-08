using UnityEngine;

[System.Serializable]
public class WeaponPartStats
{
    [Header("Identity")] 
    public string weaponPartName = "New Part";
    public WeaponType weaponType;
    public WeaponPartType WeaponPartType;
    public WeaponPartRarity WeaponPartRarity = WeaponPartRarity.Common;
}

[System.Serializable]
public enum WeaponType
{
    None,
    Swords,
    Daggers,
    Polearms,
    Clubs,
    Axes
}

[System.Serializable]
public enum WeaponPartType
{
    Handle,
    Pummel,
    Pendant,
    Guard,
    Blade
}

[System.Serializable]
public enum WeaponPartRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}