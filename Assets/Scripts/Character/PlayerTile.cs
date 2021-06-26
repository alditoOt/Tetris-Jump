using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTile : MonoBehaviour
{
    public TetriminoCell faceObject;

    public void SetTetrimino(Tetrimino tetrimino)
    {
        faceObject.gameObject.SetActive(true);
        faceObject.SetTetrimino(tetrimino);
    }
}