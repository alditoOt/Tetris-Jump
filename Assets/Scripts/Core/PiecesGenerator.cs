using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PiecesGenerator
{
    public Queue<Tetrimino> NextPieces;

    public PiecesGenerator()
    {
        StartGeneration();
    }

    public void StartGeneration()
    {
        NextPieces = new Queue<Tetrimino>();

        Tetrimino[] initialArray = GetRandomTetriminoArray();
        foreach (var tetrimino in initialArray)
        {
            Debug.Log(tetrimino);
            NextPieces.Enqueue(tetrimino);
        }
    }

    public Tetrimino PopNextPiece()
    {
        var next = NextPieces.Dequeue();
        NextPieces.Enqueue(GetRandomTetrimino());
        return next;
    }

    public List<Tetrimino> GetNextPieces(int number)
    {
        List<Tetrimino> tetriminosToShow = new List<Tetrimino>();

        foreach (var tetrimino in NextPieces)
        {
            tetriminosToShow.Add(tetrimino);
            number--;
            if (number == 0)
            {
                break;
            }
        }
        return tetriminosToShow;
    }

    private Tetrimino[] GetRandomTetriminoArray()
    {
        return ((Tetrimino[])System.Enum.GetValues(typeof(Tetrimino))).OrderBy(x => Random.Range(0, 100)).ToArray();
    }

    private Tetrimino GetRandomTetrimino()
    {
        var values = GetRandomTetriminoArray();
        return values[Random.Range(0, values.Length)];
    }
}