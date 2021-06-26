using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2 CurrentPosition;
    private Vector2 nextPosition;

    public void Start()
    {
        CurrentPosition = new Vector2(Mathf.RoundToInt(transform.localPosition.x), Mathf.RoundToInt(transform.localPosition.y));
        nextPosition = CurrentPosition;
    }

    public void Rotate(Vector2Int originPosition, bool rotationDirection)
    {
        var relativePosition = new Vector2(CurrentPosition.x, CurrentPosition.y) - originPosition;

        // rotationDirection = true when rotating clockwise
        Vector2Int xRotation = rotationDirection ? new Vector2Int(0, 1) : new Vector2Int(0, -1);
        Vector2Int yRotation = rotationDirection ? new Vector2Int(-1, 0) : new Vector2Int(1, 0);

        var newRelativePosition = new Vector2(Vector2.Dot(xRotation, relativePosition), Vector2.Dot(yRotation, relativePosition));
        nextPosition = newRelativePosition + originPosition;
    }

    public void Offset(Vector2Int offset)
    {
        nextPosition += offset;
    }

    public void Apply()
    {
        transform.DOLocalMove(nextPosition, 0.2f);
        CurrentPosition = nextPosition;
    }
}