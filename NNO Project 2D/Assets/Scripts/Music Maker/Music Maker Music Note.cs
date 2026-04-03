using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MusicNote : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [HideInInspector] public Transform parentAfterDrag;
    [SerializeField] private Image image;
    [SerializeField] private Image noteImage;
    public MusicNoteTypes note;
    
    private FlatNormalSharp flatNormalSharp = FlatNormalSharp.normal;
    
    public bool isInSpawnLocation = true;
    private Transform spawnParent;
    private MusicMakerManager manager;
    private bool canBePlayed = true;

    [SerializeField] private GameObject flatNormalSharpMenu;
    [SerializeField] private GameObject flatImage;
    [SerializeField] private GameObject sharpImage;

    void Start()
    {
        if (isInSpawnLocation) 
            spawnParent = transform.parent;
        
        manager = GetComponentInParent<MusicMakerManager>();
        manager.onPlayerStart += ResetCooldown;
    }

    void ResetCooldown() => canBePlayed = true;
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData) => transform.position = eventData.position;

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;

        if (transform.parent != spawnParent && isInSpawnLocation)
        {
            Instantiate(gameObject, spawnParent);
            isInSpawnLocation = false;
        }
    }

    void Update()
    {
        if (transform.parent == null || manager == null || manager.currentPlayer == null || !canBePlayed || isInSpawnLocation) return;

        if (manager.currentPlayer.transform.position.x > transform.position.x)
        {
            canBePlayed = false;
            PlayNote();
        }
    }

    void PlayNote()
    {
        float secondsPerBeat = 60f / manager.bpm;
        float noteDuration = secondsPerBeat;
        
        switch (note)
        {
            case MusicNoteTypes.whole:
                noteDuration = secondsPerBeat * 4f;
                break;
            case MusicNoteTypes.half:
                noteDuration = secondsPerBeat * 2f;
                break;
            case MusicNoteTypes.quarter:
                noteDuration = secondsPerBeat;
                break;
            case MusicNoteTypes.eighth:
                noteDuration = secondsPerBeat / 2f;
                break;
            case MusicNoteTypes.sixteenth:
                noteDuration = secondsPerBeat / 4f;
                break;
        }
        
        if (transform.parent.TryGetComponent(out GridSlot slot))
            slot.PlayNote(noteDuration, flatNormalSharp);

        StopAllCoroutines();
        StartCoroutine(MakeNoteGlow(noteDuration));
    }

    public IEnumerator MakeNoteGlow(float duration)
    {
        Color originalColor = Color.white;
        Image flatImg = flatImage.GetComponent<Image>();
        Image sharpImg = sharpImage.GetComponent<Image>();

        noteImage.color = Color.red;
        flatImg.color = Color.red;
        sharpImg.color = Color.red;
        
        float fadeTime = 0.2f;
        yield return new WaitForSeconds(Mathf.Max(0f, duration - fadeTime));
        
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            noteImage.color = Color.Lerp(Color.red, originalColor, t / fadeTime);
            flatImg.color = Color.Lerp(Color.red, originalColor, t / fadeTime);
            sharpImg.color = Color.Lerp(Color.red, originalColor, t / fadeTime);
            yield return null;
        }

        noteImage.color = originalColor;
        flatImg.color = originalColor;
        sharpImg.color = originalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (transform.parent == null || isInSpawnLocation) return;
        
        flatNormalSharpMenu.SetActive(!flatNormalSharpMenu.activeSelf);
    }
    
    public void SetFlat()
    {
        flatNormalSharpMenu.SetActive(false);

        flatNormalSharp = FlatNormalSharp.flat;
        flatImage.SetActive(true);
        sharpImage.SetActive(false);
    }

    public void SetNormal()
    {
        flatNormalSharpMenu.SetActive(false);
        
        flatNormalSharp = FlatNormalSharp.normal;
        flatImage.SetActive(false);
        sharpImage.SetActive(false);
    }
    
    public void SetSharp()
    {
        flatNormalSharpMenu.SetActive(false);
        
        flatNormalSharp = FlatNormalSharp.sharp;
        flatImage.SetActive(false);
        sharpImage.SetActive(true);
    }
}

public enum MusicNoteTypes
{
    whole,
    half, 
    quarter, 
    eighth,
    sixteenth
}

public enum FlatNormalSharp
{
    flat,
    normal,
    sharp
}