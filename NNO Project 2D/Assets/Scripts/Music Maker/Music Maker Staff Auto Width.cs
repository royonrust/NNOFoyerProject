using UnityEngine;
using UnityEngine.UI;

public class MusicMakerStaffAutoWidth : MonoBehaviour
{
    private HorizontalLayoutGroup layout;
    private RectTransform rectTransform;

    private void Awake()
    {
        layout = GetComponent<HorizontalLayoutGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        float width = layout.padding.left + layout.padding.right;

        foreach (RectTransform child in transform)
        {
            float scaledWidth = child.rect.width * child.localScale.x;
            width += scaledWidth + layout.spacing;
        }

        if (transform.childCount > 0)
            width -= layout.spacing;

        rectTransform.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            width
        );
    }
}