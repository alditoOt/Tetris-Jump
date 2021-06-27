using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public GameObject StartScreen;
    public TextMeshProUGUI highScore;
    private void Start()
    {
        highScore.text = GameManager.Instance.GetMaxPoints().ToString();
    }
    public void StartGame()
    {
        StartScreen.SetActive(true);
        AudioManager.Instance.Play("Tetris - Button");
        //GameManager.Instance.StartGame();
    }

    public void ResetScore()
    {
        GameManager.Instance.ResetHighScore();
        AudioManager.Instance.Play("Tetris - Button");
        highScore.text = 0.ToString();
    }

    
}
