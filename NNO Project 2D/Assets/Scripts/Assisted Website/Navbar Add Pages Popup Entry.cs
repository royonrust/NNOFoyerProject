using TMPro;
using UnityEngine;

public class NavbarAddPagesPopupEntry : MonoBehaviour
{
    [HideInInspector] public PageType page;
    private Navbar navbar;

    public void SetupText()
    {
        navbar = GetComponentInParent<Navbar>(true);
        
        var tmp = GetComponentInChildren<TextMeshProUGUI>(true);
        tmp.text = navbar.GetPageName(page);
    }
    
    public void OnPopupEntryClick()
    {
        navbar.AddPage(page);
        transform.parent.gameObject.SetActive(false);   //Disable Popup Window
    }
}