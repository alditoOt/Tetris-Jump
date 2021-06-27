using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageTetriminoCell : TetriminoCell
{
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public override void SetTetrimino(Tetrimino? tetrimino)
    {
        image.sprite = GetSprite(tetrimino);
    }
}