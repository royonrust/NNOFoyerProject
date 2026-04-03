using System;
using UnityEngine;

public class MusicMakerStaffExpander : MonoBehaviour
{
    [SerializeField] private GameObject plusButton;
    [SerializeField] private GameObject minusButton;
    [SerializeField] private GameObject staffSectionPrefab;
    [SerializeField] private Transform staffHolder;
    [SerializeField] private MusicMakerManager manager;

    void Start() => CheckAvailability();
    
    public void Plus()
    {
        Instantiate(staffSectionPrefab, staffHolder);
        transform.SetAsLastSibling();
        
        CheckAvailability();
    }
    
    public void Minus()
    {
        int targetChild = staffHolder.childCount - 2;
        Destroy(staffHolder.GetChild(targetChild).gameObject);
        
        CheckAvailability();
    }

    public void CheckAvailability()
    {
        int childCount = staffHolder.childCount;
        
        minusButton.SetActive(childCount > 3);
        plusButton.SetActive(childCount < 113);
    }
}