using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ControlPanel : MonoBehaviour
{
    [SerializeField] private int maxWidth = 18, maxHeight = 18;
    [SerializeField] private TMP_InputField fieldWidth, fieldHeight;
    [SerializeField] private GameField gameField;
    [SerializeField] private float mixTime = 2f;

    public void GenerateFieldCells()
    {
        if (string.IsNullOrEmpty(fieldWidth.text) || string.IsNullOrEmpty(fieldHeight.text))
            return;

        int width = int.Parse(fieldWidth.text);
        int height = int.Parse(fieldHeight.text);

        gameField.GenerateField(Mathf.Clamp(width, 1, maxWidth), Mathf.Clamp(height, 1, maxHeight));
    }

    public void MixCells()
    {
        gameField.MixCells(mixTime);
    }
}
