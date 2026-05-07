using UnityEngine;
using FMOD;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Slider1 : MonoBehaviour
{

    public FMOD.Studio.EventInstance EventInstance;
    public StudioEventEmitter StudioEventEmitter;
    public string Parameter;
    public UnityEngine.UI.Slider slider;

    public RawImage sliderImage;


    void Start()
    {
        StudioEventEmitter.SetParameter(Parameter, slider.value);

        
    }

    // Update is called once per frame
    void Update()
    {
       StudioEventEmitter.SetParameter(Parameter, slider.value);

        Color currentcolor = sliderImage.color;

        sliderImage.color = new Color(currentcolor.r, currentcolor.g, currentcolor.b, slider.value);
    }
}
