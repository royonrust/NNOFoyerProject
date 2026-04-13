using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NavbarButton : MonoBehaviour
{
    [SerializeField] private Button buttonComponent;
    [SerializeField] private GameObject deleteButton;
    [SerializeField] private GameObject hideButton;
    [SerializeField] private GameObject deleteConfirmation;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private TextMeshProUGUI hideButtonText;
    [SerializeField] private TextMeshProUGUI confirmationButtonText;
    
    [HideInInspector] public PageType page = PageType.home;
    private Navbar navbar;

    public void SetUpText(bool isHidden = false)
    {
        navbar = GetComponentInParent<Navbar>(true);

        if (navbar.currentlyOpenedPage == page)
            buttonText.text = navbar.GetPageName(page) + "\n[Open]";
        else
            buttonText.text = navbar.GetPageName(page);

        if (isHidden)
        {
            buttonText.text += "\n[Hidden]";
            hideButtonText.text = "Unhide";
        }

        confirmationButtonText.text = $"Yes, delete <b>{navbar.GetPageName(page)}</b>";

        bool buttonsActive = (
            GetComponentInParent<JSONToUIManager>().canEdit() &&
            page != PageType.home);
        
        deleteButton.SetActive(buttonsActive);
        hideButton.SetActive(buttonsActive);
    }

    public void NavbarButtonClick() => navbar.GoToPage(page);
    
    public void HideButtonClick() => navbar.HidePage(page);

    public void DeleteButtonClick()
    {
        buttonComponent.enabled = false;
        deleteConfirmation.SetActive(true);
    }
    
    public void CancelDelete()
    {
        buttonComponent.enabled = true;
        deleteConfirmation.SetActive(false);
    }
    
    public void ConfirmDelete() => navbar.RemovePage(page);
}