using UnityEngine;

public static class WeaponPartUtils
{
    public static Color GetRarityColor(WeaponPartRarity rar)
    {
        switch (rar)
        {
            case WeaponPartRarity.Common:
                return Color.HSVToRGB(.11f, 0f, .45f);
            case WeaponPartRarity.Uncommon:
                return Color.HSVToRGB(.11f, 0f, .75f);
            case WeaponPartRarity.Rare:
                return Color.HSVToRGB(.11f, .33f, .9f);
            case WeaponPartRarity.Epic:
                return Color.HSVToRGB(.1f, .65f, 1f);
        }
        // case WeaponPartRarity.Legendary:
        return Color.HSVToRGB(.1f, 1f, 1f);
    }
}
