using UnityEngine;

public class WeaponPart : MonoBehaviour
{
    [Header("Stats")]
    public WeaponPartStats stats = new WeaponPartStats();
    
    [Header("Model & Material")]
    public GameObject sourceFBX;
    public Material partMaterial;
    
    public Transform attachBack;
    public Transform attachFront;
    
    public Vector3 attachFrontLocalPos;
    public Vector3 attachBackLocalPos;
}