using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetriminoCell : MonoBehaviour
{
    [Header("Tetrimino Sprite")]
    public Sprite T;

    public Sprite I;
    public Sprite J;
    public Sprite L;
    public Sprite S;
    public Sprite Z;
    public Sprite O;
    public Sprite Empty;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void SetTetrimino(Tetrimino? tetrimino)
    {
        Sprite sprite = Empty;
        if (tetrimino.HasValue)
        {
            switch (tetrimino)
            {
                case Tetrimino.T:
                    sprite = T;
                    break;

                case Tetrimino.I:
                    sprite = I;
                    break;

                case Tetrimino.J:
                    sprite = J;
                    break;

                case Tetrimino.L:
                    sprite = L;
                    break;

                case Tetrimino.S:
                    sprite = S;
                    break;

                case Tetrimino.Z:
                    sprite = Z;
                    break;

                case Tetrimino.O:
                    sprite = O;
                    break;

                default:
                    break;
            }
        }
        sr.sprite = sprite;
    }
}