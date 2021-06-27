using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera vCam;
    private float shakeTimer;

    private void Start()
    {
        CameraManager.Instance.Shook.AddListener(ShakeCamera);
    }
    private void Awake()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
    }

    public void ShakeCamera(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin camPerlin = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (camPerlin != null)
        {
            StartCoroutine(ShakeCoroutine(time, camPerlin, intensity));
        }
    }

    IEnumerator ShakeCoroutine(float timer, CinemachineBasicMultiChannelPerlin camPerlin, float intensity)
    {
        camPerlin.m_AmplitudeGain = intensity;
        yield return new WaitForSeconds(timer);
        camPerlin.m_AmplitudeGain = 0f;
    }
}
