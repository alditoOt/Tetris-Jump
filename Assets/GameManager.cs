using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public GameObject screenTransition;
    public bool gameLost = false;
    private int points;

    public UnityEvent GameLost;

    private void Start()
    {
        if (GameLost == null)
        {
            GameLost = new UnityEvent();
        }
    }

    public void StartScreenTransition()
    {
        screenTransition.SetActive(true);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }


    public void EndGame()
    {
        gameLost = true;
        GameLost.Invoke();
    }

    public void SetPoints(int points)
    {
        this.points = points;
    }

    public int GetPoints()
    {
        return points;
    }
}