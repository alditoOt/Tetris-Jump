using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject StartScreen;
    public void StartGame()
    {
        StartScreen.SetActive(true);
        //GameManager.Instance.StartGame();
    }
}
