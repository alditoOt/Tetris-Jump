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
        var originPosition = GridLocator.GetGridPosition(transform, grid.transform);

        return piece.Tiles.Select(tile => new Vector2Int(Mathf.RoundToInt(tile.CurrentPosition.x), Mathf.RoundToInt(tile.CurrentPosition.y)) +
        originPosition).ToList();
    }

    public List<Vector2Int> GlobalNextTilesPositions()
    {
        var originPosition = GridLocator.GetGridPosition(transform, grid.transform);

        return piece.Tiles.Select(tile => new Vector2Int(Mathf.RoundToInt(tile.NextPosition.x), Mathf.RoundToInt(tile.NextPosition.y)) +
        originPosition).ToList();
    }
}

public static class GridLocator
{
    public static Vector2Int GetGridPosition(Transform transform, Transform grid)
    {
        return new Vector2Int(Mathf.RoundToInt(transform.position.x - grid.position.x), Mathf.RoundToInt(transform.position.y - grid.position.y));
    }
}