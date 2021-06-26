using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGrid : MonoBehaviour
{
    public Transform StartPoint;
    public Image Countdown;

    public void RespawnPiece(PlayerPiece piece, float cooldown)
    {
        piece.transform.position = StartPoint.position;
        // To-do: Handle correct initialization of pieces
        piece.Piece.InitializePiece(GetRandomTetrimino());
        var currentPoint = piece.Piece.pieceGridLocator.GlobalCurrentTilesPositions();
        var lowestPoint = piece.Piece.Grid.GetLowestPoints(currentPoint);
        var offset = lowestPoint[0].y - currentPoint[0].y;
        var newPos = new Vector2(piece.transform.position.x, piece.transform.position.y + offset);
        piece.transform.position = newPos;
        Debug.Log(newPos);
        //piece.Piece.InitializePiece(Tetrimino.O);
        Countdown.DOKill();
        Countdown.fillAmount = 1;
        Countdown.color = Color.white;
        Countdown.DOColor(Color.red, cooldown);
        Countdown.DOFillAmount(0f, cooldown);
    }

    private Tetrimino GetRandomTetrimino()
    {
        Array values = Enum.GetValues(typeof(Tetrimino));
        System.Random random = new System.Random();
        return (Tetrimino)values.GetValue(random.Next(values.Length));
    }
}