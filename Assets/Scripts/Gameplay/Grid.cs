using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class Grid : MonoBehaviour
{
    public Cell CellPrefab;
    public List<Vector2> TestBlocks = new List<Vector2>();

    private Cell[,] Cells;
    private List<Tuple<int, int>> Blocks;
    private TetrisGrid TetrisGrid;

    public UnityEvent<int> LinesChecked;
    public UnityEvent<List<int>> LinesDeleted;

    // Start is called before the first frame update
    private void Awake()
    {
        Blocks = TestBlocks.Select(block => new Tuple<int, int>(Mathf.RoundToInt(block.x), Mathf.RoundToInt(block.y))).ToList();
        TetrisGrid = new TetrisGrid(Blocks);
        Cells = new Cell[TetrisGrid.HEIGHT, TetrisGrid.WIDTH];
        for (int i = 0; i < TetrisGrid.HEIGHT; i++)
        {
            for (int j = 0; j < TetrisGrid.WIDTH; j++)
            {
                Cells[i, j] = Instantiate(CellPrefab, transform);
                Cells[i, j].transform.localPosition = new Vector3(j, i, transform.position.z);
                if (i >= TetrisGrid.VISUAL_HEIGHT)
                {
                    Cells[i, j].SetInvisible();
                }
            }
        }
    }

    private void Start()
    {
        if (LinesChecked == null)
        {
            LinesChecked = new UnityEvent<int>();
        }
        if (LinesDeleted == null)
        {
            LinesDeleted = new UnityEvent<List<int>>();
        }
        RenderGrid();
        GameManager.Instance.GameLost.AddListener(OnGameLost);
    }

    private void OnGameLost()
    {
    }

    public void CheckForLines(List<Vector2Int> modifiedBlocks)
    {
        List<int> completeLinesIndexes = TetrisGrid.CheckForLines(modifiedBlocks);

        TetrisGrid.DestroyLines(completeLinesIndexes);
        LinesChecked.Invoke(completeLinesIndexes.Count);
        LinesDeleted.Invoke(completeLinesIndexes);
    }

    private void RenderGrid()
    {
        for (int i = 0; i < TetrisGrid.HEIGHT; i++)
        {
            for (int j = 0; j < TetrisGrid.WIDTH; j++)
            {
                RenderCell(j, i, TetrisGrid.GetCell(j, i));
            }
        }
    }

    private void RenderCell(int x, int y, TetrisCell cell)
    {
        if (cell.Present)
        {
            Cells[y, x].SetBlock(cell.Tetrimino);
        }
        else if (cell.Preview)
        {
            Cells[y, x].SetPreview(cell.Tetrimino);
        }
        else
        {
            Cells[y, x].Clear();
        }
    }

    public void SetNextPreview(Tetrimino nextPiece, Transform startPoint)
    {
        var startPosition = GridLocator.GetGridPosition(startPoint, transform);
        var nextPreview = new List<Vector2Int>();
        var layout = TetriminoLogic.Layouts[nextPiece];
        foreach (var tile in layout)
        {
            nextPreview.Add(startPosition + tile);
        }
        TetrisGrid.SetNextPreview(nextPreview);
    }

    public void UpdatePreview(Piece piece)
    {
        var isDifferent = TetrisGrid.AddPreviewBlocks(piece.pieceGridLocator.GlobalCurrentTilesPositions(), piece.Tetrimino);
        if (isDifferent)
        {
            RenderGrid();
        }
    }

    public void PlaceBlocks(Piece piece)
    {
        var modifiedBlocks = TetrisGrid.AddBlocks(piece.pieceGridLocator.GlobalCurrentTilesPositions(), piece.Tetrimino);
        CheckForLines(modifiedBlocks);
        RenderGrid();
    }

    public bool IsSpaceAvailable(List<Vector2Int> positions)
    {
        return TetrisGrid.CheckSpace(positions);
    }

    public List<Vector2Int> GetLowestPoints(List<Vector2Int> positions)
    {
        return TetrisGrid.GetLowestPoints(positions);
    }
}

public class TetrisGrid
{
    public static readonly int WIDTH = 10;
    public static readonly int HEIGHT = 25;
    public static readonly int VISUAL_HEIGHT = 20;

    private TetrisCell[,] Cells;
    private HashSet<Vector2Int> previews;
    private HashSet<Vector2Int> nextPreview;

    public TetrisGrid(List<Tuple<int, int>> blocks = null)
    {
        Cells = new TetrisCell[HEIGHT, WIDTH];
        previews = new HashSet<Vector2Int>();
        nextPreview = new HashSet<Vector2Int>();

        for (int i = 0; i < HEIGHT; i++)
        {
            for (int j = 0; j < WIDTH; j++)
            {
                Cells[i, j] = new TetrisCell();
            }
        }

        if (blocks != null)
        {
            // Initialize test data
            foreach (var block in blocks)
            {
                if (block.Item1 >= 0 && block.Item2 >= 0
                    && block.Item1 < WIDTH && block.Item2 < HEIGHT)
                {
                    Cells[block.Item2, block.Item1].SetValue(null);
                }
            }
        }
    }

    public List<Vector2Int> AddBlocks(List<Vector2Int> points, Tetrimino tetrimino)
    {
        if (IsOutOfBounds(points))
        {
            // Do nothing
            return new List<Vector2Int>();
        }

        ClearPreviews();

        var lowest = GetLowestPoints(points);
        foreach (var point in lowest)
        {
            Cells[point.y, point.x].SetValue(tetrimino);
        }
        return lowest;
    }

    public bool AddPreviewBlocks(List<Vector2Int> points, Tetrimino tetrimino)
    {
        if (IsOutOfBounds(points))
        {
            // Do nothing
            return false;
        }

        var lowest = GetLowestPoints(points);

        if (previews.Count == 0 || !previews.All(lowest.Contains))
        {
            ClearPreviews();
            foreach (var point in lowest)
            {
                Cells[point.y, point.x].SetPreview(tetrimino);
                previews.Add(point);
            }

            // Preview
            var previewLowest = GetLowestPoints(nextPreview, true);
            foreach (var point in previewLowest)
            {
                previews.Add(point);
                Cells[point.y, point.x].SetPreview(null);
            }
            return true;
        }

        return false;
    }

    public bool CheckSpace(List<Vector2Int> points)
    {
        return points.All(point =>
        {
            return !IsOutOfBounds(point.x, point.y) && !Cells[point.y, point.x].Present;
        });
    }

    public TetrisCell GetCell(int x, int y)
    {
        return Cells[y, x];
    }

    public List<int> CheckForLines(List<Vector2Int> modifiedBlocks)
    {
        List<int> completeLinesIndexes = new List<int>();
        foreach (var modifiedY in modifiedBlocks.Select(m => m.y).Distinct())
        {
            bool isComplete = true;
            for (int j = 0; j < WIDTH; j++)
            {
                if (!Cells[modifiedY, j].Present)
                {
                    isComplete = false;
                    break;
                }
            }
            if (isComplete)
            {
                completeLinesIndexes.Add(modifiedY);
            }
        }
        return completeLinesIndexes;
    }

    public void DestroyLines(List<int> indexes)
    {
        foreach (var index in indexes.OrderByDescending(i => i))
        {
            for (int j = 0; j < WIDTH; j++)
            {
                Cells[index, j].Clear();
            }
            for (int i = index + 1; i < HEIGHT; i++)
            {
                for (int j = 0; j < WIDTH; j++)
                {
                    if (Cells[i, j].Present)
                    {
                        Cells[i - 1, j] = Cells[i, j].Copy();
                        Cells[i, j].Clear();
                    }
                }
            }
        }
    }

    public TetrisGrid Copy()
    {
        var cellsCopy = new TetrisCell[HEIGHT, WIDTH];

        for (int i = 0; i < HEIGHT; i++)
        {
            for (int j = 0; j < WIDTH; j++)
            {
                cellsCopy[i, j] = Cells[i, j].Copy();
            }
        }
        return new TetrisGrid()
        {
            Cells = cellsCopy
        };
    }

    public void ClearPreviews()
    {
        foreach (var preview in previews)
        {
            Cells[preview.y, preview.x].Clear();
        }
        previews.Clear();
    }

    private bool IsOutOfBounds(int x, int y)
    {
        return x < 0 || y < 0
                || x >= WIDTH || y >= HEIGHT;
    }

    private bool IsOutOfBounds(List<Vector2Int> points)
    {
        return points.Any(point => IsOutOfBounds(point.x, point.y));
    }

    public List<Vector2Int> GetLowestPoints(IEnumerable<Vector2Int> points, bool includePreviews = false)
    {
        var offset = 0;
        while (offset < HEIGHT)
        {
            var hasCollided = false;
            foreach (var point in points)
            {
                if (point.y - offset <= 0 || Cells[point.y - offset - 1, point.x].Present ||
                    (includePreviews && Cells[point.y - offset - 1, point.x].Preview))
                {
                    hasCollided = true;
                    break;
                }
            }
            if (hasCollided)
            {
                break;
            }
            offset++;
        }
        return points.Select(point => new Vector2Int(point.x, point.y - offset)).ToList();
    }

    public void SetNextPreview(List<Vector2Int> points)
    {
        nextPreview = new HashSet<Vector2Int>(points);
    }
}

public class TetrisCell
{
    public bool Present { get; set; }
    public bool Preview { get; set; }
    public Tetrimino? Tetrimino { get; set; }

    public void SetPreview(Tetrimino? tetrimino)
    {
        Present = false;
        Preview = true;
        Tetrimino = tetrimino;
    }

    public void SetValue(Tetrimino? tetrimino)
    {
        Present = true;
        Preview = false;
        Tetrimino = tetrimino;
    }

    public void Clear()
    {
        Present = false;
        Preview = false;
        Tetrimino = null;
    }

    public TetrisCell Copy()
    {
        return new TetrisCell()
        {
            Present = Present,
            Preview = Preview,
            Tetrimino = Tetrimino
        };
    }

    public override bool Equals(object obj)
    {
        //Check for null and compare run-time types.
        if ((obj == null) || !GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            TetrisCell p = (TetrisCell)obj;
            return Present == p.Present && Preview == p.Preview && Tetrimino.Equals(p.Tetrimino);
        }
    }

    public override int GetHashCode()
    {
        int hashCode = 367770434;
        hashCode = hashCode * -1521134295 + Present.GetHashCode();
        hashCode = hashCode * -1521134295 + Tetrimino.GetHashCode();
        hashCode = hashCode * -1521134295 + Preview.GetHashCode();
        return hashCode;
    }
}