using UnityEngine;

public class MinigamesMenuManager : MonoBehaviour
{
    void Start() => CheckAvailability();
    
    [SerializeField] private GameObject musicMakerBanner;
    
    public void CheckAvailability()
    {
        GeneralManager general = GetComponentInParent<GeneralManager>();
        musicMakerBanner.SetActive(general.hasPhysicalPanel);
    }
    
    public void CloseSelf() => gameObject.SetActive(false);
    
    public void OpenMusicMaker()
    {
        var menu = transform.parent.GetComponentInChildren<MusicMakerManager>(true);
        if (menu)
        {
            menu.gameObject.SetActive(true);
            CloseSelf();
        }
    }
}