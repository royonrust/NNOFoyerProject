using TMPro;
using UnityEngine;

public class DisplaysActivater : MonoBehaviour
{
    
    void Start()
    {
        bool secondDisplayActivated = false;

        if (Display.displays.Length > 1)
        {
            Display.displays[1].Activate();

            // Required on Windows — assign camera to the second display
            Camera[] allCameras = Camera.allCameras;
            if (allCameras.Length > 1)
                allCameras[1].targetDisplay = 1;

            secondDisplayActivated = true;
        }

        string displayAmount = Display.displays.Length.ToString();
        string secondActive = secondDisplayActivated ? "yes" : "no";

    }
}