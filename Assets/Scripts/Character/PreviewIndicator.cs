using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public class PreviewIndicator : MonoBehaviour
{
    public PlayerPiece player;
    private Grid grid;

    private void Start()
    {
        grid = GetComponent<Grid>();
    }

    // Update is called once per frame
    private void Update()
    {
        grid.UpdatePreview(player.Piece);
    }
}