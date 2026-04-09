using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UITextblock : UIElementBase, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public TextMeshProUGUI tmp;
    public TMP_InputField inputField;
    public CanvasGroup canvasGroup;
    private bool canEdit;
    
    public override UIElementData GenerateData()
    {
        return new UITextblockData
        {
            prefabID = prefabID,
            anchoredPosition = ((RectTransform)transform).anchoredPosition,
            parentIdentifier = parentName,
            text = tmp.text
        };
    }

    public override void ApplyData(UIElementData baseData)
    {
        var d = (UITextblockData)baseData;
        
        tmp.text = d.text;
        inputField.text = d.text;
    }

    private void Start()
    {
        canEdit = GetComponentInParent<JSONToUIManager>().canEdit();
        inputField.text = tmp.text;
    }
    
    public void SetTextFromInputField()
    {
        tmp.text = inputField.text;
        inputField.gameObject.SetActive(false);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!canEdit) return;
        transform.position = eventData.position;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!canEdit) return;
        inputField.gameObject.SetActive(true);
        inputField.ActivateInputField();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canEdit) return;
        canvasGroup.blocksRaycasts = false;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!canEdit) return;
        canvasGroup.blocksRaycasts = true;
    }
    
}

[Serializable]
public class UITextblockData : UIElementData
{
    public string text;
}