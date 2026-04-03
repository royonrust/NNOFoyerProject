using UnityEngine;
using UnityEngine.EventSystems;

public class MusicMakerGarbageCan : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        if (dropped.TryGetComponent(out MusicNote note)) 
            Destroy(note.gameObject);
    }
}