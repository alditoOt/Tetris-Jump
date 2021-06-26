using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2 CurrentPosition;
    public Vector2 NextPosition;
    public Transform Collider;
    public PlayerBlockDisplay Display;

    public void Start()
    {
        InitializeValues();
    }

    public void InitializeValues()
    {
        CurrentPosition = new Vector2(Mathf.RoundToInt(Collider.localPosition.x), Mathf.RoundToInt(Collider.localPosition.y));
        NextPosition = CurrentPosition;
    }

    public void Rotate(Vector2Int originPosition, bool rotationDirection)
    {
        var relativePosition = new Vector2(CurrentPosition.x, CurrentPosition.y) - originPosition;

        // rotationDirection = true when rotating clockwise
        Vector2Int xRotation = rotationDirection ? new Vector2Int(0, 1) : new Vector2Int(0, -1);
        Vector2Int yRotation = rotationDirection ? new Vector2Int(-1, 0) : new Vector2Int(1, 0);

        var newRelativePosition = new Vector2(Vector2.Dot(xRotation, relativePosition), Vector2.Dot(yRotation, relativePosition));
        NextPosition = newRelativePosition + originPosition;
    }

    public void Offset(Vector2Int offset)
    {
        NextPosition += offset;
    }

    public void Undo()
    {
        NextPosition = CurrentPosition;
    }

    public void Apply()
    {
        Collider.localPosition = NextPosition;
        Display.MoveToPosition(NextPosition);
        CurrentPosition = NextPosition;
    }
}