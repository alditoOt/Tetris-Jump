using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPiece : MonoBehaviour
{
    public Piece Piece;
    public PlayerGrid Grid;
    public float cooldown = 10f;
    public float cooldownModifier = 0.7f;

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
        float currentCooldown = Mathf.Max(cooldown - Piece.Grid.totalLines / 10 * cooldownModifier, 3f);
        Grid.RespawnPiece(this, currentCooldown);
        yield return new WaitForSeconds(currentCooldown);
        OnPlace();
    }
}