using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrentPlayingWidget : MonoBehaviour
{
    private StudioEventEmitter studioEventEmitter;
    private Slider slider;
    
    [SerializeField] private TextMeshProUGUI currentPositionTMP;
    [SerializeField] private TextMeshProUGUI lengthTMP;

    private float positionSeconds;
    private float lengthSeconds;

    void Start()
    {
        slider = GetComponentInChildren<Slider>();
        studioEventEmitter = GetComponent<StudioEventEmitter>();
        
        studioEventEmitter.EventInstance.getTimelinePosition(out int positionMs);
        studioEventEmitter.EventInstance.getDescription(out FMOD.Studio.EventDescription description);
        description.getLength(out int lengthMs);
        
        positionSeconds = positionMs / 1000f;
        lengthSeconds = lengthMs / 1000f;
        
        slider.maxValue = lengthSeconds;
        slider.value = positionSeconds;
        
        SetSeconds(true);
    }

    void Update()
    {
        studioEventEmitter.EventInstance.getTimelinePosition(out int positionMs);
        positionSeconds = positionMs / 1000f;
        
        slider.value = positionSeconds;

        SetSeconds();
    }

    void SetSeconds(bool includeLength = false)
    {
        currentPositionTMP.text = FormatTime(positionSeconds);
        if (includeLength) lengthTMP.text = FormatTime(lengthSeconds);
    }

    string FormatTime(float seconds)
    {
        int m = Mathf.FloorToInt(seconds / 60f);
        int s = Mathf.FloorToInt(seconds % 60f);
        return $"{m}:{s:D2}";
    }
}