using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNextPieces : MonoBehaviour
{
    public Transform nextPiecesContainer;

    public void SetNextPieces(List<Tetrimino> tetriminos)
    {
        var nextPiecesUI = nextPiecesContainer.GetComponentsInChildren<TetriminoCell>();
        for (int i = 0; i < nextPiecesUI.Length; i++)
        {
            nextPiecesUI[i].SetTetrimino(tetriminos[i]);
        }
    }
}