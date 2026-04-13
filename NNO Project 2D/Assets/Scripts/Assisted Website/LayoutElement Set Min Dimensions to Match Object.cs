using UnityEngine;
using UnityEngine.UI;

public class LayoutElementSetMinDimenionsToMatchObject : MonoBehaviour
{
    private LayoutElement layoutElement;
    [SerializeField] private RectTransform targetTransform;
    [SerializeField] private bool copyHeight;
    [SerializeField] private bool copyWidth;

    private void Start() => layoutElement = GetComponent<LayoutElement>();

    private void Update()
    {
        if (copyHeight) layoutElement.minHeight = targetTransform.rect.height;
        if (copyWidth) layoutElement.minWidth = targetTransform.rect.width;
    }
}