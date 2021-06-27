using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public GameObject endScreen;

    private void Start()
    {
        GameManager.Instance.GameLost.AddListener(() => endScreen.SetActive(true));
    }
}