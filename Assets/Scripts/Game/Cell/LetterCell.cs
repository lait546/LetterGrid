using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LetterCell : MonoBehaviour
{
    public RectTransform RectTransform;
    public LetterCellMovement Movement;
    public TextMeshProUGUI Letter;
    [SerializeField] private AspectRatioFitter aspectRatioFitter;
    [HideInInspector] public GridCell GridCell;

    public void Init(string _letter, GridCell _cell)
    {
        Letter.text = _letter;
        GridCell = _cell;
    }

    public void ChangeAcpectMode(AspectRatioFitter.AspectMode mode)
    {
        aspectRatioFitter.aspectMode = mode;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
