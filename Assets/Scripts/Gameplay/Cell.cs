using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public GameObject Block;
    public GameObject Preview;

    public void SetBlock()
    {
        Block.gameObject.SetActive(true);
        Preview.gameObject.SetActive(false);
    }

    public void SetPreview()
    {
        Block.gameObject.SetActive(false);
        Preview.gameObject.SetActive(true);
    }

    public void Clear()
    {
        Block.gameObject.SetActive(false);
        Preview.gameObject.SetActive(false);
    }
}