using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class GameField : MonoBehaviour
{
    [SerializeField] private GridCell gridCellPref;
    [SerializeField] private LetterCell letterCellPref;
    [SerializeField] private Transform cellsContainer, wordsContainer;
    [SerializeField] private CustomLayout customLayout;
    [SerializeField] private List<GridCell> cells = new List<GridCell>();

    private int width, height;
    private string words = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    //Вызывается из инспектора
    public void GenerateField(int _width, int _height)
    {
        width = _width;
        height = _height;

        foreach(GridCell cell in cells.ToList())
        {
            cell.Destroy();
            cells.Remove(cell);
        }

        customLayout.constraintCount = width;

        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                GridCell cell = Instantiate(gridCellPref, cellsContainer);
                cells.Add(cell);
            }

        //Нужен из за специфики работы layout group.
        //Layout group не успевает за один кадр рассчитать позиции ячеек, нужно пропустить один кадр.
        StartCoroutine(ICreateWords());
    }

    private IEnumerator ICreateWords()
    {
        yield return null;
        for (int i = 0; i < cells.Count; i++)
        {
            LetterCell letter = Instantiate(letterCellPref, cells[i].transform.position, Quaternion.identity, cells[i].transform);
            letter.transform.SetParent(wordsContainer.transform);
            letter.Init(words[Random.Range(0, words.Length)].ToString(), cells[i]);
            letter.RectTransform.sizeDelta = cells[i].rectTransform.sizeDelta;
            cells[i].SetLetterCell(letter);
            ChangeAspectModeCellLetter(letter);
        }
    }

    private void ChangeAspectModeCellLetter(LetterCell _letter)
    {
        if (width > height)
            _letter.ChangeAcpectMode(UnityEngine.UI.AspectRatioFitter.AspectMode.WidthControlsHeight);
        else
            _letter.ChangeAcpectMode(UnityEngine.UI.AspectRatioFitter.AspectMode.HeightControlsWidth);
    }

    //Вызывается из инспектора
    public void MixCells(float mixTime)
    {
        if (cells.Count.Equals(0))
            return;

        cells.Shuffle();

        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].transform.SetSiblingIndex(cells.IndexOf(cells[i]));
            cells[i].letter.Movement.UpdatePosition(mixTime);
        }
    }
}
