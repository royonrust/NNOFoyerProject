using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    public WeaponPartRegistry registry;

    public void SpawnWeapon()
    {
        WeaponPart handle = GetRandomPart(WeaponPartType.Handle, WeaponType.Swords);
        WeaponPart guard = GetRandomPart(WeaponPartType.Guard, WeaponType.Swords);
        WeaponPart blade = GetRandomPart(WeaponPartType.Blade, WeaponType.Swords);
        WeaponPart pummel = GetRandomPart(WeaponPartType.Pummel, WeaponType.Swords);
        WeaponPart pendant = GetRandomPart(WeaponPartType.Pendant, WeaponType.None);

        WeaponPart spawnedHandle = SpawnPart(handle, transform.position, transform);

        WeaponPart spawnedGuard = SpawnPart(guard, GetAttachFrontWorld(spawnedHandle), spawnedHandle.transform);
        SpawnPart(blade, GetAttachFrontWorld(spawnedGuard), spawnedGuard.transform);

        WeaponPart spawnedPummel = SpawnPart(pummel, GetAttachBackWorld(spawnedHandle), spawnedHandle.transform);
        SpawnPart(pendant, GetAttachFrontWorld(spawnedPummel), spawnedPummel.transform);
    }

    WeaponPart SpawnPart(WeaponPart foundPart, Vector3 position, Transform parent)
    {
        GameObject spawnedGO = Instantiate(foundPart.gameObject, position, Quaternion.identity, parent);
        return spawnedGO.GetComponent<WeaponPart>();
    }

    Vector3 GetAttachFrontWorld(WeaponPart spawnedPart)
    {
        return spawnedPart.transform.TransformPoint(spawnedPart.attachFrontLocalPos);
    }

    Vector3 GetAttachBackWorld(WeaponPart spawnedPart)
    {
        return spawnedPart.transform.TransformPoint(spawnedPart.attachBackLocalPos);
    }
    
    WeaponPart GetRandomPart(WeaponPartType partType, WeaponType weaponType)
    {
        bool hasCommon = false;
        bool hasUncommon = false;
        bool hasRare = false;
        bool hasEpic = false;
        bool hasLegendary = false;

        List<WeaponPart> availableParts = new List<WeaponPart>();
        foreach(WeaponPart part in registry.parts)
            if (part.stats.WeaponPartType == partType && part.stats.weaponType == weaponType)
            {
                availableParts.Add(part);
                switch (part.stats.WeaponPartRarity)
                {
                    case WeaponPartRarity.Common:
                        hasCommon = true;
                        break;
                    case WeaponPartRarity.Uncommon:
                        hasUncommon = true;
                        break;
                    case WeaponPartRarity.Rare:
                        hasRare = true;
                        break;
                    case WeaponPartRarity.Epic:
                        hasEpic = true;
                        break;
                    case WeaponPartRarity.Legendary:
                        hasLegendary = true;
                        break;
                }
            }

        return GetRarityWeighedPart(availableParts, hasCommon, hasUncommon, hasRare, hasEpic, hasLegendary);
    }

    WeaponPart GetRarityWeighedPart(List<WeaponPart> list, bool common, bool uncommon, bool rare, bool epic, bool legendary)
    {
        float totalWeight = 0f;
        if (common)    totalWeight += 50f;
        if (uncommon)  totalWeight += 25f;
        if (rare)      totalWeight += 15f;
        if (epic)      totalWeight += 8f;
        if (legendary) totalWeight += 2f;

        float roll = Random.Range(0f, totalWeight);

        WeaponPartRarity selectedRarity;
        float cumulative = 0f;

        if (common    && roll < (cumulative += 50f)) selectedRarity = WeaponPartRarity.Common;
        else if (uncommon  && roll < (cumulative += 25f)) selectedRarity = WeaponPartRarity.Uncommon;
        else if (rare      && roll < (cumulative += 15f)) selectedRarity = WeaponPartRarity.Rare;
        else if (epic      && roll < (cumulative += 8f))  selectedRarity = WeaponPartRarity.Epic;
        else                                              selectedRarity = WeaponPartRarity.Legendary;

        List<WeaponPart> rarityPool = new List<WeaponPart>();
        foreach (WeaponPart part in list)
            if (part.stats.WeaponPartRarity == selectedRarity)
                rarityPool.Add(part);

        return rarityPool[Random.Range(0, rarityPool.Count)];
    }
}