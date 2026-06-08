using UnityEngine;
using FMODUnity;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))] [RequireComponent(typeof(StudioEventEmitter))]
public class Slider1 : MonoBehaviour
{
    private StudioEventEmitter studioEventEmitter;
    private Slider slider;
    private string parameter;
    private CanvasGroup sectionCanvasGroup;
    private TextMeshProUGUI percentageText;

    private void Start()
    {
        slider = GetComponent<Slider>();
        studioEventEmitter = GetComponent<StudioEventEmitter>();
        parameter = studioEventEmitter.Params[0].Name;
        percentageText = GetComponentInChildren<TextMeshProUGUI>(true);
        
        GameObject sectionObject = GameObject.Find("Section Visual " + gameObject.name.Replace("Slider ", ""));
        
        if (sectionObject == null)
        {
            Debug.LogWarning(
                $"No orchestra canvas group found for: {gameObject.name} \n" + 
                $"Please check object names to ensure the below pattern is being followed! \n \n" +
                $"Slider object name: Slider XX \n" +
                $"Section object name: Section Visual XX \n");
        } 
        else sectionCanvasGroup = sectionObject.GetComponent<CanvasGroup>();
        
        studioEventEmitter.SetParameter(parameter, slider.value);
        ChangePercentageText();
    }

    public void OnSliderValueChanged()
    {
        studioEventEmitter.SetParameter(parameter, slider.value);
        ChangePercentageText();


        if (sectionCanvasGroup == null)
            return;

        sectionCanvasGroup.alpha = Mathf.Pow(slider.value / 100f, 2.5f);
    }

    private void ChangePercentageText() => percentageText.text = $"{slider.value}%";
}
