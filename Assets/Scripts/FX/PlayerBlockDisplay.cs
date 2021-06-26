using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlockDisplay : MonoBehaviour
{
    public TetriminoCell Face;

    internal void MoveToPosition(Vector2 nextPosition)
    {
        transform.DOLocalMove(nextPosition, 0.2f);
    }
}