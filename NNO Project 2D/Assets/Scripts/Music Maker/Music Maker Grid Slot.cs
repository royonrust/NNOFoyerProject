using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private AudioClip flatClip;
    [SerializeField] private AudioClip normalClip;
    [SerializeField] private AudioClip sharpClip;
    private MusicMakerManager manager;
    
    void Start() => manager = GetComponentInParent<MusicMakerManager>();
    
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount > 0) return;
        
        GameObject dropped = eventData.pointerDrag;
        if (dropped.TryGetComponent(out MusicNote note)) 
            note.parentAfterDrag = transform;
    }

    public void PlayNote(float duration, FlatNormalSharp fns)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = normalClip;
        
        switch (fns)
        {
            case FlatNormalSharp.flat:
                source.clip = flatClip;
                break;
            case FlatNormalSharp.sharp:
                source.clip = sharpClip;
                break;
        }
        
        source.Play();
        StartCoroutine(FadeAndDestroy(source, duration, 0.2f));
    }

    IEnumerator FadeAndDestroy(AudioSource source, float duration, float fadeTime)
    {
        yield return new WaitForSeconds(duration - fadeTime);

        float startVolume = source.volume;

        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, t / fadeTime);
            yield return null;
        }

        source.Stop();
        Destroy(source);
    }
}