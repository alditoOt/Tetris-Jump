using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class Grid : MonoBehaviour
{
    public Cell CellPrefab;
    public List<Vector2> TestBlocks = new List<Vector2>();
    public PlayerGrid pg;

    private Cell[,] Cells;
    private List<Tuple<int, int>> Blocks;
    private TetrisGrid TetrisGrid;
    private TetrisGrid PreviousTetrisGrid;
    private int Combo = 0;
    private int points = 0;
    public int totalLines = 0;
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI linesAmount;
    public TextMeshProUGUI tetrisFX;
    public TextMeshProUGUI single;
    public TextMeshProUGUI doubleLine;
    public TextMeshProUGUI triple;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI finalScore;

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
        Combo = 0;
    }

    private void Start()
    {
        RenderGrid();
    }

    private void FixedUpdate()
    {
        if(pg.gameLost)
        {
            finalScore.text = Convert.ToString(points);
        }
    }
    public void CheckForLines()
    {
        List<int> completeLinesIndexes = TetrisGrid.CheckForLines();
        DestroyLines(completeLinesIndexes);
    }

    private void DestroyLines(List<int> indexes)
    {
        int numberOfLines = indexes.Count;
        if (numberOfLines == 0)
        {
            Combo = 0;
        }
        else
        {
            Combo++;
            PreviousTetrisGrid = TetrisGrid.Copy();
            switch (numberOfLines)
            {
                case 1:
                    points += 40 + 50*(Combo-1);
                    totalLines += 1;
                    single.gameObject.SetActive(true);
                    break;
                case 2:
                    points += 100 + 50 * (Combo - 1);
                    totalLines += 2;
                    doubleLine.gameObject.SetActive(true);
                    break;
                case 3:
                    points += 300 + 50 * (Combo - 1);
                    totalLines += 3;
                    triple.gameObject.SetActive(true);
                    break;
                case 4:
                    points += 1200 + 50 * (Combo - 1);
                    totalLines += 4;
                    tetrisFX.gameObject.SetActive(true);
                    break;
                default:
                    break;
            }
            linesAmount.text = Convert.ToString(totalLines);
            pointsText.text = Convert.ToString(points);
            Debug.Log(points);
            TetrisGrid.DestroyLines(indexes);
            RenderGrid();
            PreviousTetrisGrid = null;
        }
        if (Combo < 1)
        {
            comboText.text = Convert.ToString(0);
        }
        else
        {
            comboText.text = Convert.ToString(Combo - 1);
        }
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

    public void UpdatePreview(List<Vector2Int> positions, Tetrimino tetrimino)
    {
        PreviousTetrisGrid = TetrisGrid.Copy();
        TetrisGrid.ClearPreviews();
        TetrisGrid.SetPoints(positions, tetrimino, false, true);
        RenderGrid();
        PreviousTetrisGrid = null;
    }

    public void PlaceBlocks(List<Vector2Int> positions, Tetrimino tetrimino)
    {
        PreviousTetrisGrid = TetrisGrid.Copy();
        TetrisGrid.ClearPreviews();
        TetrisGrid.SetPoints(positions, tetrimino, true, false);
        RenderGrid();
        CheckForLines();
        PreviousTetrisGrid = null;
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
        if (IsOutOfBounds(points))
        {
            // Do nothing
            return;
        }

        var lowest = GetLowestPoints(points);
        foreach (var point in lowest)
        {
            if (isPreview && !Cells[point.y, point.x].Present)
            {
                Cells[point.y, point.x].SetPreview(tetrimino);
            }
            else if (isPresent)
            {
                Cells[point.y, point.x].SetValue(tetrimino);
            }
        }
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

    private bool IsOutOfBounds(int x, int y)
    {
        return x < 0 || y < 0
                || x >= WIDTH || y >= HEIGHT;
    }

    private bool IsOutOfBounds(List<Vector2Int> points)
    {
        return points.Any(point => IsOutOfBounds(point.x, point.y));
    }

    public List<Vector2Int> GetLowestPoints(List<Vector2Int> points)
    {
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
        return points.Select(point => new Vector2Int(point.x, point.y - offset)).ToList();
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