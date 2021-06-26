using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPiece : MonoBehaviour
{
    public Piece Piece;
    public PlayerGrid Grid;

    private Coroutine spawningRoutine;

    private void Start()
    {
        spawningRoutine = StartCoroutine(SpawnedPiece());
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
        Piece.Grid.PlaceBlocks(Piece.pieceGridLocator.GlobalCurrentTilesPositions(), Piece.Tetrimino);
        StopCoroutine(spawningRoutine);
        spawningRoutine = StartCoroutine(SpawnedPiece());
    }

    private IEnumerator SpawnedPiece()
    {
        Grid.RespawnPiece(this, 3f);
        yield return new WaitForSeconds(3f);
        OnPlace();
    }
}