using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    public RectTransform rectTransform;
    public LetterCell letter;

    public void SetLetterCell(LetterCell _letter)
    {
        letter = _letter;
    }

    public void Destroy()
    {
        letter.Destroy();
        Destroy(gameObject);
    }
}
