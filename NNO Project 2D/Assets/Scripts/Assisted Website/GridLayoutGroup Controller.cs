using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class GridLayoutGroupController : MonoBehaviour
{
    [SerializeField][Range(1, 5)] private int horizontalCellAmount = 2;

    private void Start() => UpdateLayout();

    private void OnRectTransformDimensionsChange() => UpdateLayout();
    
    private void OnValidate() => UpdateLayout();

    private void UpdateLayout()
    {
        RectTransform rect = GetComponent<RectTransform>();
        GridLayoutGroup layout = GetComponent<GridLayoutGroup>();   

        float totalWidth = rect.rect.width;
        float spacing = layout.spacing.x;
        float padding = layout.padding.horizontal;
        float cellWidth = (totalWidth - spacing - padding) / horizontalCellAmount;

        layout.cellSize = new Vector2(cellWidth, layout.cellSize.y);
    }
}