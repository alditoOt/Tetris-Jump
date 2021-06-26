using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PieceGridLocator
{
    public Grid grid;
    public Piece piece;
    public Transform transform;

    public List<Vector2Int> GlobalCurrentTilesPositions()
    {
        var originPosition = new Vector2Int(Mathf.RoundToInt(transform.position.x - grid.transform.position.x), Mathf.RoundToInt(transform.position.y - grid.transform.position.y));

        return piece.Tiles.Select(tile => new Vector2Int(Mathf.RoundToInt(tile.CurrentPosition.x), Mathf.RoundToInt(tile.CurrentPosition.y)) +
        originPosition).ToList();
    }

    public List<Vector2Int> GlobalNextTilesPositions()
    {
        var originPosition = new Vector2Int(Mathf.RoundToInt(transform.position.x - grid.transform.position.x), Mathf.RoundToInt(transform.position.y - grid.transform.position.y));

        return piece.Tiles.Select(tile => new Vector2Int(Mathf.RoundToInt(tile.NextPosition.x), Mathf.RoundToInt(tile.NextPosition.y)) +
        originPosition).ToList();
    }
}