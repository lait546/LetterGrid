using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomLayout : LayoutGroup
{
    [SerializeField] private int rows;
    [SerializeField] private int columns;
    [SerializeField] private Vector2 cellSize;

    /// <summary>
    /// Which corner is the starting corner for the grid.
    /// </summary>
    public enum Corner
    {
        /// <summary>
        /// Upper Left corner.
        /// </summary>
        UpperLeft = 0,
        /// <summary>
        /// Upper Right corner.
        /// </summary>
        UpperRight = 1,
        /// <summary>
        /// Lower Left corner.
        /// </summary>
        LowerLeft = 2,
        /// <summary>
        /// Lower Right corner.
        /// </summary>
        LowerRight = 3
    }

    /// <summary>
    /// The grid axis we are looking at.
    /// </summary>
    /// <remarks>
    /// As the storage is a [][] we make access easier by passing a axis.
    /// </remarks>
    public enum Axis
    {
        /// <summary>
        /// Horizontal axis
        /// </summary>
        Horizontal = 0,
        /// <summary>
        /// Vertical axis.
        /// </summary>
        Vertical = 1
    }

    /// <summary>
    /// Constraint type on either the number of columns or rows.
    /// </summary>
    public enum Constraint
    {
        /// <summary>
        /// Don't constrain the number of rows or columns.
        /// </summary>
        Flexible = 0,
        /// <summary>
        /// Constrain the number of columns to a specified number.
        /// </summary>
        FixedColumnCount = 1,
        /// <summary>
        /// Constraint the number of rows to a specified number.
        /// </summary>
        FixedRowCount = 2
    }

    [SerializeField] protected Corner m_StartCorner = Corner.UpperLeft;

    /// <summary>
    /// Which corner should the first cell be placed in?
    /// </summary>
    public Corner startCorner { get { return m_StartCorner; } set { SetProperty(ref m_StartCorner, value); } }

    [SerializeField] protected Axis m_StartAxis = Axis.Horizontal;

    /// <summary>
    /// Which axis should cells be placed along first
    /// </summary>
    /// <remarks>
    /// When startAxis is set to horizontal, an entire row will be filled out before proceeding to the next row. When set to vertical, an entire column will be filled out before proceeding to the next column.
    /// </remarks>
    public Axis startAxis { get { return m_StartAxis; } set { SetProperty(ref m_StartAxis, value); } }

    [SerializeField] protected Vector2 m_Spacing = Vector2.zero;

    /// <summary>
    /// The spacing to use between layout elements in the grid on both axises.
    /// </summary>
    public Vector2 spacing { get { return m_Spacing; } set { SetProperty(ref m_Spacing, value); } }

    [SerializeField] protected Constraint m_Constraint = Constraint.Flexible;

    /// <summary>
    /// Which constraint to use for the GridLayoutGroup.
    /// </summary>
    /// <remarks>
    /// Specifying a constraint can make the GridLayoutGroup work better in conjunction with a [[ContentSizeFitter]] component. When GridLayoutGroup is used on a RectTransform with a manually specified size, there's no need to specify a constraint.
    /// </remarks>
    public Constraint constraint { get { return m_Constraint; } set { SetProperty(ref m_Constraint, value); } }

    [SerializeField] protected int m_ConstraintCount = 2;

    /// <summary>
    /// How many cells there should be along the constrained axis.
    /// </summary>
    public int constraintCount { get { return m_ConstraintCount; } set { SetProperty(ref m_ConstraintCount, Mathf.Max(1, value)); } }

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
    }

    /// <summary>
    /// Called by the layout system to calculate the vertical layout size.
    /// Also see ILayoutElement
    /// </summary>
    public override void CalculateLayoutInputVertical()
    {

    }

    public override void SetLayoutHorizontal()
    {
        SetCellsAlongAxis(0);
    }

    public override void SetLayoutVertical()
    {
        SetCellsAlongAxis(1);
    }

    private void SetCellsAlongAxis(int axis)
    {
        if (m_Constraint == Constraint.FixedColumnCount)
        {
            rows = transform.childCount / constraintCount;
            columns = constraintCount;
        }
        else if (m_Constraint == Constraint.FixedRowCount)
        {
            rows = constraintCount;
            columns = transform.childCount / constraintCount;
        }

        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        float cellWidth = parentWidth / (float)columns;
        float cellHeight = parentHeight / (float)rows;

        cellSize.x = cellWidth;
        cellSize.y = cellHeight;

        int columtCount = 0;
        int rowCount = 0;

        // Normally a Layout Controller should only set horizontal values when invoked for the horizontal axis
        // and only vertical values when invoked for the vertical axis.
        // However, in this case we set both the horizontal and vertical position when invoked for the vertical axis.
        // Since we only set the horizontal position and not the size, it shouldn't affect children's layout,
        // and thus shouldn't break the rule that all horizontal layout must be calculated before all vertical layout.
        var rectChildrenCount = rectChildren.Count;
        if (axis == 0)
        {
            // Only set the sizes when invoked for horizontal axis, not the positions.

            for (int i = 0; i < rectChildrenCount; i++)
            {
                RectTransform rect = rectChildren[i];

                m_Tracker.Add(this, rect,
                    DrivenTransformProperties.Anchors |
                    DrivenTransformProperties.AnchoredPosition |
                    DrivenTransformProperties.SizeDelta);

                rect.anchorMin = Vector2.up;
                rect.anchorMax = Vector2.up;
                rect.sizeDelta = cellSize;
            }
            return;
        }

        float width = rectTransform.rect.size.x;
        float height = rectTransform.rect.size.y;

        int cellCountX = 1;
        int cellCountY = 1;
        if (m_Constraint == Constraint.FixedColumnCount)
        {
            cellCountX = m_ConstraintCount;

            if (rectChildrenCount > cellCountX)
                cellCountY = rectChildrenCount / cellCountX + (rectChildrenCount % cellCountX > 0 ? 1 : 0);
        }
        else if (m_Constraint == Constraint.FixedRowCount)
        {
            cellCountY = m_ConstraintCount;

            if (rectChildrenCount > cellCountY)
                cellCountX = rectChildrenCount / cellCountY + (rectChildrenCount % cellCountY > 0 ? 1 : 0);
        }
        else
        {
            if (cellSize.x + spacing.x <= 0)
                cellCountX = int.MaxValue;
            else
                cellCountX = Mathf.Max(1, Mathf.FloorToInt((width - padding.horizontal + spacing.x + 0.001f) / (cellSize.x + spacing.x)));

            if (cellSize.y + spacing.y <= 0)
                cellCountY = int.MaxValue;
            else
                cellCountY = Mathf.Max(1, Mathf.FloorToInt((height - padding.vertical + spacing.y + 0.001f) / (cellSize.y + spacing.y)));
        }

        int cornerX = (int)startCorner % 2;
        int cornerY = (int)startCorner / 2;

        int cellsPerMainAxis, actualCellCountX, actualCellCountY;
        if (startAxis == Axis.Horizontal)
        {
            cellsPerMainAxis = cellCountX;
            actualCellCountX = Mathf.Clamp(cellCountX, 1, rectChildrenCount);
            actualCellCountY = Mathf.Clamp(cellCountY, 1, Mathf.CeilToInt(rectChildrenCount / (float)cellsPerMainAxis));
        }
        else
        {
            cellsPerMainAxis = cellCountY;
            actualCellCountY = Mathf.Clamp(cellCountY, 1, rectChildrenCount);
            actualCellCountX = Mathf.Clamp(cellCountX, 1, Mathf.CeilToInt(rectChildrenCount / (float)cellsPerMainAxis));
        }

        Vector2 requiredSpace = new Vector2(
            actualCellCountX * cellSize.x + (actualCellCountX - 1) * spacing.x,
            actualCellCountY * cellSize.y + (actualCellCountY - 1) * spacing.y
        );
        Vector2 startOffset = new Vector2(
            GetStartOffset(0, requiredSpace.x),
            GetStartOffset(1, requiredSpace.y)
        );

        for (int i = 0; i < rectChildrenCount; i++)
        {
            var item = rectChildren[i];

            if (startAxis == Axis.Horizontal)
            {
                rowCount = i % (cellsPerMainAxis);
                columtCount = i / (cellsPerMainAxis);
            }
            else
            {
                rowCount = i / (cellsPerMainAxis / columns);
                columtCount = i % (cellsPerMainAxis / columns);
            }

            if (cornerX == 1)
            {
                rowCount = actualCellCountX - 1 - rowCount;
            }
            if (cornerY == 1)
            {
                columtCount = actualCellCountY - 1 - columtCount;
            }

            SetChildAlongAxis(item, 0, startOffset.x + cellSize.x  * rowCount, cellSize.x);
            SetChildAlongAxis(item, 1, startOffset.y + cellSize.y  * columtCount, cellSize.y);
        }
    }
}
