using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerPiece : MonoBehaviour
{
    public Grid Grid;

    [HideInInspector]
    public Piece Piece;

    public UnityEvent PiecePlaced;

    private void Start()
    {
        if (PiecePlaced == null)
        {
            PiecePlaced = new UnityEvent();
        }
        Piece = GetComponentInChildren<Piece>();
    }

    public void OnRotateLeft()
    {
        Piece.Rotate(false);
    }

    public void OnRotateRight()
    {
        Piece.Rotate(true);
    }

    public void OnPlace()
    {
        Grid.PlaceBlocks(Piece);
        PiecePlaced.Invoke();
    }
}