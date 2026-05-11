using TMPro;
using UnityEngine;

public class DisplaysActivater : MonoBehaviour
{
    public TextMeshProUGUI tmp;
    
    void Start()
    {
        bool secondDisplayActived = false;
        
        if (Display.displays.Length > 1)
        {
            Display.displays[1].Activate();
            secondDisplayActived = true;
        }

        string displayAmount = Display.displays.Length.ToString();
        string secondActive = secondDisplayActived ? "yes" : "no";
        
        tmp.text = $"Displays found: {displayAmount} \n" +
                   $"Second display active: {secondActive}";
    }
}