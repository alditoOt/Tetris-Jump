using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public TetriminoCell Block;
    public TetriminoCell Preview;
    public bool Visible = true;

    public void SetBlock(Tetrimino? tetrimino)
    {
        if (!Visible)
        {
            return;
        }
        Block.gameObject.SetActive(true);
        Block.SetTetrimino(tetrimino);
        Preview.gameObject.SetActive(false);
    }

    public void SetPreview(Tetrimino? tetrimino)
    {
        if (!Visible)
        {
            return;
        }
        Block.gameObject.SetActive(false);
        Preview.gameObject.SetActive(true);
        Preview.SetTetrimino(tetrimino);
    }

    public void Clear()
    {
        Block.gameObject.SetActive(false);
        Preview.gameObject.SetActive(false);
    }

    public void SetInvisible()
    {
        Visible = false;
        Clear();
        GetComponent<SpriteRenderer>().enabled = false;
    }
}