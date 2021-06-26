using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Cell CellPrefab;
    public List<Vector2> TestBlocks = new List<Vector2>();

    private Cell[,] Cells;
    private List<Tuple<int, int>> Blocks;
    private TetrisGrid TetrisGrid;
    private TetrisGrid PreviousTetrisGrid;

    // Start is called before the first frame update
    private void Start()
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
            }
        }
        RenderGrid();
        CheckForLines();
    }

    public void CheckForLines()
    {
        List<int> completeLinesIndexes = TetrisGrid.CheckForLines();
        DestroyLines(completeLinesIndexes);
    }

    private void DestroyLines(List<int> indexes)
    {
        int numberOfLines = indexes.Count;
        PreviousTetrisGrid = TetrisGrid.Copy();
        // TO-DO: Handle points for different number of completed lines
        // 1: SIMPLE
        // 2: DOUBLE
        // 3: TRIPLE
        // 4: TETRIS OMG
        TetrisGrid.DestroyLines(indexes);
        RenderGrid();
        PreviousTetrisGrid = null;
    }

    private void RenderGrid()
    {
        for (int i = 0; i < TetrisGrid.HEIGHT; i++)
        {
            for (int j = 0; j < TetrisGrid.WIDTH; j++)
            {
                RenderCell(j, i, TetrisGrid.GetCell(j, i), PreviousTetrisGrid?.GetCell(j, i));
            }
        }
    }

    private void RenderCell(int x, int y, TetrisCell cell, TetrisCell previousCell)
    {
        if (previousCell == null || (!cell.Equals(previousCell)))
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
    }

    public void LocatePlayer(List<Vector2Int> positions, Tetrimino tetrimino)
    {
        TetrisGrid.ClearPreviews();
        TetrisGrid.SetPoints(positions, tetrimino, false, true);
        RenderGrid();
    }

    public bool TestPlayer(List<Vector2Int> positions)
    {
        return TetrisGrid.CheckSpace(positions);
    }
}

public class TetrisGrid
{
    public static readonly int WIDTH = 10;
    public static readonly int HEIGHT = 20;

    private TetrisCell[,] Cells;

    public TetrisGrid(List<Tuple<int, int>> blocks = null)
    {
        Cells = new TetrisCell[HEIGHT, WIDTH];

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

    public void SetPoints(List<Vector2Int> points, Tetrimino tetrimino, bool isPresent = false, bool isPreview = false)
    {
        foreach (var point in points)
        {
            if (point.x < 0 || point.y < 0
                || point.x >= WIDTH || point.y >= HEIGHT)
            {
                return;
            }
        }
        var offset = 0;
        while (offset < HEIGHT)
        {
            var hasCollided = false;
            foreach (var point in points)
            {
                if (point.y - offset <= 0 || Cells[point.y - offset - 1, point.x].Present)
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
        foreach (var point in points)
        {
            if (isPreview && !Cells[point.y - offset, point.x].Present)
            {
                Cells[point.y - offset, point.x].SetPreview(tetrimino);
            }
            else if (isPresent)
            {
                Cells[point.y - offset, point.x].SetValue(tetrimino);
            }
        }
    }

    public bool CheckSpace(List<Vector2Int> points)
    {
        return points.All(point =>
        {
            return point.x >= 0 && point.y >= 0
                && point.x < WIDTH && point.y < HEIGHT && !Cells[point.y, point.x].Present;
        });
    }

    public TetrisCell GetCell(int x, int y)
    {
        return Cells[y, x];
    }

    public List<int> CheckForLines()
    {
        List<int> completeLinesIndexes = new List<int>();
        for (int i = 0; i < HEIGHT; i++)
        {
            bool isComplete = true;
            for (int j = 0; j < WIDTH; j++)
            {
                if (!Cells[i, j].Present)
                {
                    isComplete = false;
                    break;
                }
            }
            if (isComplete)
            {
                completeLinesIndexes.Add(i);
            }
        }
        return completeLinesIndexes;
    }

    public void DestroyLines(List<int> indexes)
    {
        foreach (var index in indexes)
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
        for (int i = 0; i < HEIGHT; i++)
        {
            for (int j = 0; j < WIDTH; j++)
            {
                if (Cells[i, j].Preview)
                {
                    Cells[i, j].Clear();
                }
            }
        }
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