using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraManager : MonoBehaviourSingleton<CameraManager>
{
    public UnityEvent<float, float> Shook;

    private void Start()
    {
        if(Shook == null)
        {
            Shook = new UnityEvent<float, float>();
        }
    }
    public void Shake(float intensity, float time)
    {
        Shook.Invoke(intensity, time);
    }
}
