using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public GameObject screenTransition;
    public bool gameLost = false;

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
}

    public void EndGame()
    {
        gameLost = true;
        GameLost.Invoke();
    }
}