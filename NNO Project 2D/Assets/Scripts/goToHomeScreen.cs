using UnityEngine;

public class goToHomeScreen : MonoBehaviour
{
    [SerializeField] private Transform screensHolder;

    public void GoToHomeScreen()
    {
        foreach (Transform activeScreen in screensHolder) 
            activeScreen.gameObject.SetActive(false);

        MainMenuManager homeScreen = transform.root.GetComponentInChildren<MainMenuManager>(true);
        homeScreen.CheckAvailability();
        homeScreen.gameObject.SetActive(true);
    }
}