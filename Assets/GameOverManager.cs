using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public GameObject endScreen;
    public TextMeshProUGUI finalScore;
    public TextMeshProUGUI HighScore;

    private void Start()
    {
        GameManager.Instance.GameLost.AddListener(OnGameOver);
    }

    public void Restart()
    {
        GameManager.Instance.RestartScene();
    }

    public void BackToMenu()
    {
        GameManager.Instance.RestartGame();
    }

  
    public void OnGameOver()
    {
        Debug.Log("GameOver");
        endScreen.SetActive(true);
        finalScore.text = GameManager.Instance.GetPoints().ToString();
        GameManager.Instance.SetMaxPoints(GameManager.Instance.GetPoints());
        HighScore.text = GameManager.Instance.GetMaxPoints().ToString();
    }
}