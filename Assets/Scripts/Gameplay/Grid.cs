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
                Cells[y, x].SetBlock();
            }
            else
            {
                Cells[y, x].Clear();
            }
        }
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
                    Cells[block.Item2, block.Item1].Present = true;
                }
            }
        }
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
}

public class TetrisCell
{
    public bool Present { get; set; }

    public void Clear()
    {
        Present = false;
    }

    public TetrisCell Copy()
    {
        return new TetrisCell()
        {
            Present = this.Present
        };
    }
}