using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterCellMovement : MonoBehaviour
{
    [SerializeField] private LetterCell letterCell;

    public void UpdatePosition(float animateTime)
    {
        StartCoroutine(IUpdatePosition(animateTime));
    }

    private IEnumerator IUpdatePosition(float animateTime)
    {
        float startTime = Time.realtimeSinceStartup, fraction = 0f;
        Vector2 curPos = transform.position;
        while (fraction < 1f)
        {
            fraction = Mathf.Clamp01((Time.realtimeSinceStartup - startTime) / animateTime);
            transform.position = Vector3.Lerp(curPos, letterCell.GridCell.transform.position, fraction);
            yield return new WaitForFixedUpdate();
        }
    }
}
