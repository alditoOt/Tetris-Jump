using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteTetriminoCell : TetriminoCell
{
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public override void SetTetrimino(Tetrimino? tetrimino)
    {
        sr.sprite = GetSprite(tetrimino);
    }
}