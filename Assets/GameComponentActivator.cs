using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameComponentActivator : MonoBehaviour
{
    public void DeactivateObject()
    {
        this.gameObject.SetActive(false);
    }
}
