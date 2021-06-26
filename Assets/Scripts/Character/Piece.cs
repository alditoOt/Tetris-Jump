using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Tetrimino Tetrimino;
    public Tile TilePrefab;
    public Grid Grid;

    [HideInInspector]
    public List<Tile> Tiles = new List<Tile>();

    private int rotationIndex = 0;
    public PieceGridLocator pieceGridLocator;

    private void Awake()
    {
        pieceGridLocator = new PieceGridLocator()
        {
            grid = Grid,
            piece = this,
            transform = transform
        };
    }

    public void InitializePiece(Tetrimino tetrimino)
    {
        Tetrimino = tetrimino;
        rotationIndex = 0;
        var layout = TetriminoLogic.Layouts[tetrimino];
        for (int i = 0; i < Tiles.Count; i++)
        {
            if (Tiles[i] != null)
            {
                Destroy(Tiles[i].gameObject);
            }
        }
        Tiles = new List<Tile>();
        for (int i = 0; i < layout.Length; i++)
        {
            var tile = Instantiate(TilePrefab, transform);
            tile.Collider.localPosition = new Vector3(layout[i].x, layout[i].y, transform.position.z);
            tile.Display.transform.localPosition = new Vector3(layout[i].x, layout[i].y, transform.position.z);
            tile.Display.GetComponent<TetriminoCell>().SetTetrimino(tetrimino);
            tile.InitializeValues();
            Tiles.Add(tile);
        }

        // Add face to pivot
        Tiles[0].Display.Face.gameObject.SetActive(true);
        Tiles[0].Display.Face.SetTetrimino(tetrimino);

        // Set ground checkers in parent
        transform.parent.GetComponent<PlayerMovement>()
            .SetCheckers(Tiles.SelectMany(tile => tile.GetComponent<PlayerTile>().groundCheckers).ToList());
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
        var originPosition = new Vector2Int(Mathf.RoundToInt(Tiles[0].CurrentPosition.x), Mathf.RoundToInt(Tiles[0].CurrentPosition.y));
        for (int i = 1; i < Tiles.Count; i++)
        {
            Tiles[i].Rotate(originPosition, rotationDirection);
        }
        var success = Offset(oldRotationIndex, rotationIndex);
        for (int i = 0; i < Tiles.Count; i++)
        {
            if (success)
            {
                Tiles[i].Apply();
            }
            else
            {
                Tiles[i].Undo();
            }
        }
    }

    private bool Offset(int oldRotationIndex, int newRotationIndex)
    {
        if (Tetrimino == Tetrimino.O)
        {
            var offset = TetriminoLogic.OPieceOffsetData[oldRotationIndex] - TetriminoLogic.OPieceOffsetData[newRotationIndex];
            for (int i = 0; i < Tiles.Count; i++)
            {
                Tiles[i].Offset(offset);
            }
            return true;
        }
        else
        {
            var offsetData = TetriminoLogic.DefaultOffsetData;
            if (Tetrimino == Tetrimino.I)
            {
                offsetData = TetriminoLogic.IPieceOffsetData;
            }
            for (int c = 0; c < 5; c++)
            {
                var offset = offsetData[oldRotationIndex, c] - offsetData[newRotationIndex, c];
                for (int i = 0; i < Tiles.Count; i++)
                {
                    Tiles[i].Offset(offset);
                }
                if (Grid.IsSpaceAvailable(pieceGridLocator.GlobalNextTilesPositions()))
                {
                    return true;
                }
                for (int i = 0; i < Tiles.Count; i++)
                {
                    Tiles[i].Offset(-offset);
                }
            }
        }
        return false;
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
        [Tetrimino.J] = new Vector2Int[4] { new Vector2Int(1, 0), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(2, 0) },
        [Tetrimino.Single] = new Vector2Int[1] { new Vector2Int(0, 0) }
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