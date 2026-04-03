using UnityEngine;
using UnityEngine.UI;

public class MusicMakerHorizontalLineEnabler : MonoBehaviour
{
    [SerializeField] private Transform[] notes;
    private Image image;

    void Start() => image = GetComponent<Image>();

    void Update()
    {
        bool enable = false;
        
        foreach (Transform obj in notes)
            if (obj.childCount > 0)
                enable = true;

        image.enabled = enable;
    }
}