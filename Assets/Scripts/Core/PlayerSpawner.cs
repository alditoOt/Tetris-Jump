using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public PlayerPiece Player;
    public Transform StartPoint;

    private Grid Grid;
    private PlayerLifespan Lifespan;
    private PlayerNextPieces NextPieces;
    private PlayerStats Stats;
    private PiecesGenerator Generator;

    private void Awake()
    {
        Generator = new PiecesGenerator();
    }

    private void Start()
    {
        Lifespan = GetComponent<PlayerLifespan>();
        NextPieces = GetComponent<PlayerNextPieces>();
        Stats = GetComponent<PlayerStats>();
        Grid = GetComponentInChildren<Grid>();
        Player.PiecePlaced.AddListener(SpawnPlayer);
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        var nextPiece = Generator.PopNextPiece();
        NextPieces.SetNextPieces(Generator.GetNextPieces(4));
        // Move the piece to starting position
        Player.transform.position = StartPoint.position;
        Player.Piece.InitializePiece(nextPiece);
        MoveToLowestPoint();

        var height = Player.transform.position.y - Grid.transform.position.y;
        if (height >= TetrisGrid.VISUAL_HEIGHT)
        {
            GameManager.Instance.EndGame();
        }

        Lifespan.StartPieceLifespan(Player, Stats);

        nextPiece = Generator.GetNextPieces(1)[0];
        Grid.SetNextPreview(nextPiece, StartPoint);
    }

    private void MoveToLowestPoint()
    {
        var currentPoint = Player.Piece.pieceGridLocator.GlobalCurrentTilesPositions();
        var lowestPoint = Grid.GetLowestPoints(currentPoint);

        var offset = lowestPoint[0].y - currentPoint[0].y;
        var newPos = new Vector2(Player.transform.position.x, Player.transform.position.y + offset);
        Player.transform.position = newPos;
    }

    private void SetNextPreview()
    {
    }
}