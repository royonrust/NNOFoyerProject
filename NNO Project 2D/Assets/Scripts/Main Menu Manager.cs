using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    void Start() => CheckAvailability();
    
    [SerializeField] private GameObject panelMenuBanner;
    
    public void CheckAvailability()
    {
        GeneralManager general = GetComponentInParent<GeneralManager>();
        panelMenuBanner.SetActive(general.hasPhysicalPanel);
    }
    
    public void CloseSelf() => gameObject.SetActive(false);
    
    public void OpenNNOMenu()
    {
        var nnoMenu = transform.parent.GetComponentInChildren<NNOMenuManager>(true);
        if (nnoMenu)
        {
            nnoMenu.gameObject.SetActive(true);
            CloseSelf();
        }
    }

    public void OpenPanelMenu()
    {
        var panelMenu = transform.parent.GetComponentInChildren<PanelMenuManager>(true);
        if (panelMenu)
        {
            panelMenu.gameObject.SetActive(true);
            CloseSelf();
        }
    }
    
    public void OpenMinigamesMenu()
    {
        var minigamesMenu = transform.parent.GetComponentInChildren<MinigamesMenuManager>(true);
        if (minigamesMenu)
        {
            minigamesMenu.CheckAvailability();
            
            minigamesMenu.gameObject.SetActive(true);
            CloseSelf();
        }
    }
}