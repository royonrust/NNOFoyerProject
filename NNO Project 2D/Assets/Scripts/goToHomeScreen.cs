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
    
    //temp vv

    public GameObject buttonDown;
    public GameObject buttonUp;
    public RectTransform entireObject;
    private float moveAmount = 180f;

    public void MoveDown()
    {
        buttonDown.SetActive(false);
        buttonUp.SetActive(true);
        Vector2 pos = entireObject.anchoredPosition;
        pos.y -= moveAmount;
        entireObject.anchoredPosition = pos;
    }

    public void MoveUp()
    {
        buttonDown.SetActive(true);
        buttonUp.SetActive(false);
        Vector2 pos = entireObject.anchoredPosition;
        pos.y += moveAmount;
        entireObject.anchoredPosition = pos;
    }
}