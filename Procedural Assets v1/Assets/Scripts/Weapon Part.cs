using UnityEngine;

public class WeaponPart : MonoBehaviour
{
    [Header("Part Stats")]
    public WeaponPartStats stats = new WeaponPartStats();
 
    [Header("Attachment Points")]
    [Tooltip("Where the PREVIOUS part in the chain connects to this one (e.g. where a blade receives a guard).")]
    public Transform attachBack;
    [Space]
 
    [Tooltip("Where the NEXT part in the chain connects (e.g. where a blade connects to a tip).")]
    public Transform attachFront;
}