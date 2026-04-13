using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UITextblock : DraggableUIElement
{
    [SerializeField] private TextMeshProUGUI tmp;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private CanvasGroup canvasGroup;

    public override UIElementData GenerateData()
    {
        return new UITextblockData
        {
            prefabID = prefabID,
            anchoredPosition = ((RectTransform)transform).anchoredPosition,
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
    
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (!canEdit) return;
        inputField.gameObject.SetActive(true);
        inputField.ActivateInputField();
    }
    
    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (!canEdit) return;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        
        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(500, 350);
        
        canvasGroup.blocksRaycasts = false;
    }
    
    public override void OnEndDrag(PointerEventData eventData)
    {
        if (!canEdit) return;
        transform.SetParent(parentAfterDrag);
        canvasGroup.blocksRaycasts = true;
    }
}

[Serializable]
public class UITextblockData : UIElementData
{
    public string text;
}