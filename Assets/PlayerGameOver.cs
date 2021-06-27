using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGameOver : MonoBehaviour
{
    private PlayerInput playerInput;
    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        GameManager.Instance.GameLost.AddListener(() => {
            playerInput.enabled = false;
        });
    }
}
