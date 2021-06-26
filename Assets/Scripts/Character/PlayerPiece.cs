using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPiece : MonoBehaviour
{
    public Piece Piece;

    public void OnRotateLeft()
    {
        Piece.Rotate(false);
    }

    public void OnRotateRight()
    {
        Piece.Rotate(true);
    }
}