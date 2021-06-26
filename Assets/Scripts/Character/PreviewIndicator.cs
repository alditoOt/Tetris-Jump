using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Piece))]
public class PreviewIndicator : MonoBehaviour
{
    private Piece piece;

    private void Start()
    {
        piece = GetComponent<Piece>();
    }

    // Update is called once per frame
    private void Update()
    {
        piece.grid.LocatePlayer(piece.pieceGridLocator.GlobalCurrentTilesPositions(), piece.Tetrimino);
    }
}