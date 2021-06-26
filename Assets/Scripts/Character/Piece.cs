using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Tetrimino Tetrimino;
    public Tile TilePrefab;
    private int rotationIndex = 0;
    private Tile[] tiles = new Tile[4];

    private void Start()
    {
        InitializePiece(Tetrimino);
    }

    public void InitializePiece(Tetrimino tetrimino)
    {
        var layout = TetriminoLogic.Layouts[tetrimino];
        for (int i = 0; i < 4; i++)
        {
            tiles[i] = Instantiate(TilePrefab, transform);
            tiles[i].transform.localPosition = new Vector3(layout[i].x, layout[i].y, transform.position.z);
            tiles[i].GetComponent<TetriminoCell>().SetTetrimino(tetrimino);
            if (i == 0)
            {
                tiles[i].GetComponent<PlayerTile>().SetTetrimino(tetrimino);
            }
        }
    }

    public void Rotate(bool rotationDirection)
    {
        var oldRotationIndex = rotationIndex;
        if (rotationDirection)
        {
            // Clockwise
            rotationIndex = (rotationIndex + 1) % 4;
        }
        else
        {
            // Counter Clockwise
            rotationIndex = (rotationIndex + 3) % 4;
        }
        var originPosition = new Vector2Int(Mathf.RoundToInt(tiles[0].CurrentPosition.x), Mathf.RoundToInt(tiles[0].CurrentPosition.y));
        for (int i = 1; i < 4; i++)
        {
            tiles[i].Rotate(originPosition, rotationDirection);
        }
        var success = Offset(oldRotationIndex, rotationIndex);
        if (!success)
        {
            Rotate(!rotationDirection);
            return;
        }
        for (int i = 0; i < 4; i++)
        {
            tiles[i].Apply();
        }
    }

    private bool Offset(int oldRotationIndex, int newRotationIndex)
    {
        if (Tetrimino == Tetrimino.O)
        {
            var offset = TetriminoLogic.OPieceOffsetData[oldRotationIndex] - TetriminoLogic.OPieceOffsetData[newRotationIndex];
            Debug.Log(offset);
            for (int i = 0; i < 4; i++)
            {
                tiles[i].Offset(offset);
            }
            return true;
        }
        else if (Tetrimino == Tetrimino.I)
        {
            var offset = TetriminoLogic.IPieceOffsetData[oldRotationIndex, 0] - TetriminoLogic.IPieceOffsetData[newRotationIndex, 0];
            for (int i = 0; i < 4; i++)
            {
                tiles[i].Offset(offset);
            }
        }
        else
        {
            var offset = TetriminoLogic.DefaultOffsetData[oldRotationIndex, 0] - TetriminoLogic.DefaultOffsetData[newRotationIndex, 0];
            for (int i = 0; i < 4; i++)
            {
                tiles[i].Offset(offset);
            }
        }
        return true;
    }
}

public static class TetriminoLogic
{
    // First piece is the pivot
    public static readonly Dictionary<Tetrimino, Vector2Int[]> Layouts = new Dictionary<Tetrimino, Vector2Int[]>()
    {
        [Tetrimino.T] = new Vector2Int[4] { new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(0, 0), new Vector2Int(1, 1) },
        [Tetrimino.L] = new Vector2Int[4] { new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(0, 0), new Vector2Int(2, 1) },
        [Tetrimino.S] = new Vector2Int[4] { new Vector2Int(1, 0), new Vector2Int(2, 1), new Vector2Int(0, 0), new Vector2Int(1, 1) },
        [Tetrimino.Z] = new Vector2Int[4] { new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 0) },
        [Tetrimino.I] = new Vector2Int[4] { new Vector2Int(1, 0), new Vector2Int(0, 0), new Vector2Int(2, 0), new Vector2Int(3, 0) },
        [Tetrimino.O] = new Vector2Int[4] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) },
        [Tetrimino.J] = new Vector2Int[4] { new Vector2Int(1, 0), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(2, 0) }
    };

    public static readonly Vector2Int[,] DefaultOffsetData = new Vector2Int[4, 5] {
        {  new Vector2Int(0, 0), new Vector2Int(0, 0), new Vector2Int(0, 0), new Vector2Int(0, 0), new Vector2Int(0, 0) },
        {  new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, -1), new Vector2Int(0, 2), new Vector2Int(1, 2) },
        {  new Vector2Int(0, 0), new Vector2Int(0, 0), new Vector2Int(0, 0), new Vector2Int(0, 0), new Vector2Int(0, 0) },
        {  new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, -1), new Vector2Int(0, 2), new Vector2Int(-1, 2) }
    };

    public static readonly Vector2Int[,] IPieceOffsetData = new Vector2Int[4, 5] {
        {  new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(2, 0), new Vector2Int(-1, 0), new Vector2Int(2, 0) },
        {  new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, -2) },
        {  new Vector2Int(-1, 1), new Vector2Int(1, 1), new Vector2Int(-2, 1), new Vector2Int(1, 0), new Vector2Int(-2, 0) },
        {  new Vector2Int(0, 1), new Vector2Int(0, 1), new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(0, 2) }
    };

    public static readonly Vector2Int[] OPieceOffsetData = new Vector2Int[4] {
        new Vector2Int(0, 0) ,
        new Vector2Int(0, -1) ,
        new Vector2Int(-1, -1) ,
        new Vector2Int(-1, 0)
    };
}