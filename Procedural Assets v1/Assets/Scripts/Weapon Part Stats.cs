using UnityEngine;

[System.Serializable]
public class WeaponPartStats
{
    [Header("Identity")] 
    public string partName = "New Part";
    public WeaponType weaponType;
    public WeaponPartType WeaponPartType;
    
    [Header("Rarity")]
    public WeaponPartRarity WeaponPartRarity = WeaponPartRarity.Common;
}

[System.Serializable]
public enum WeaponType
{
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
    HandleMaterial,
    Pummel,
    Pendant,
    HandleGuard,
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