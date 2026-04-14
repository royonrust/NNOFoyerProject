using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NavbarAddPagesButton : MonoBehaviour
{
    private Navbar navbar;

    [SerializeField] private GameObject popupWindow;
    [SerializeField] private GameObject popupWindowEntry;
    
    private void Start() => navbar = GetComponentInParent<Navbar>();

    public void OpenPopupWindow()
    {
        List<PageType> missingPageTypes = Enum.GetValues(typeof(PageType))
            .Cast<PageType>()
            .Where(p => p != PageType.empty && !navbar.pageTypes.Contains(p))
            .ToList();
            
        foreach (Transform child in popupWindow.transform)
            Destroy(child.gameObject);
        
        foreach (var page in missingPageTypes)
            SpawnPopupEntry(page);
        
        popupWindow.SetActive(!popupWindow.activeSelf);
    }

    private void SpawnPopupEntry(PageType page)
    {
        var newEntry = Instantiate(popupWindowEntry, popupWindow.transform);
        var entryScript = newEntry.GetComponent<NavbarAddPagesPopupEntry>();
        
        entryScript.page = page;
        entryScript.SetupText();
    }
}