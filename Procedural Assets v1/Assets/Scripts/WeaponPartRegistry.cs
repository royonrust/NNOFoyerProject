using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon Parts/Part Registry")]
public class WeaponPartRegistry : ScriptableObject
{
    public List<WeaponPart> parts = new List<WeaponPart>();
}