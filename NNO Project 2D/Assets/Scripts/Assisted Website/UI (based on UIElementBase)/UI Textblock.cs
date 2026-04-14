using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class UITextblock : DraggableUIElement
{
    private TextMeshProUGUI tmp;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private InputFieldFormatter formatter;
    private CanvasGroup canvasGroup;

    [SerializeField] private bool draggable;

    public override UIElementData GenerateData()
    {
        tmp ??= GetComponentInChildren<TextMeshProUGUI>();
        
        var d = new UITextblockData
        {
            text = tmp.text,
            draggable = draggable,
            fontSize = tmp.fontSizeMax,
            fontStyle = tmp.fontStyle,
            alignment = tmp.alignment
        };
        PopulateIDAndRectData(d);
        return d;
    }

    public override void ApplyCustomData(UIElementData baseData)
    {
        tmp ??= GetComponentInChildren<TextMeshProUGUI>();
        var d = (UITextblockData)baseData;

        inputField.text = d.text;
        tmp.text = d.text;
        draggable = d.draggable;
        ApplyFontSize(d.fontSize);
        ApplyFontStyle(d.fontStyle);
        ApplyAlignment(d.alignment);
    }

    private void Start()
    {
        tmp ??= GetComponentInChildren<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();

        canEdit = GetComponentInParent<JSONToUIManager>().canEdit();
        isDraggable = draggable;
        inputField.text = tmp.text;
        ApplyFontSize(tmp.fontSizeMax);
        ApplyFontStyle(tmp.fontStyle);
        ApplyAlignment(tmp.alignment);
    }

    public void SetTextFromInputField()
    {
        tmp.text = inputField.text;
        tmp.fontSizeMax = inputField.textComponent.fontSizeMax;
        tmp.fontStyle = inputField.textComponent.fontStyle;
        tmp.alignment = inputField.textComponent.alignment;
        inputField.gameObject.SetActive(false);
    }

    private void ApplyFontSize(float size)
    {
        tmp.enableAutoSizing = true;
        tmp.fontSizeMin = 6;
        tmp.fontSizeMax = size;
        formatter.ApplyFontSize(size);
    }

    private void ApplyFontStyle(FontStyles style)
    {
        tmp.fontStyle = style;
        formatter.ApplyFontStyle(style);
    }

    private void ApplyAlignment(TextAlignmentOptions alignment)
    {
        tmp.alignment = alignment;
        formatter.ApplyAlignment(alignment);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (!canEdit) return;
        inputField.gameObject.SetActive(true);
        inputField.ActivateInputField();
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;

        if (!canEdit || !draggable) return;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();

        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(500, 350);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        if (!canEdit || !draggable) return;
        transform.SetParent(parentAfterDrag);
        RefreshPageDimensions();
    }
}

[Serializable]
public class UITextblockData : UIElementData
{
    public bool draggable;
    public string text;
    public float fontSize;
    public FontStyles fontStyle;
    public TextAlignmentOptions alignment;
}