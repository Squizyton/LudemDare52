using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CinemachineScreenshake : MonoBehaviour
{
    private CinemachineVirtualCamera cineCamera;

    private float shakeTimer;

    // Start is called before the first frame update
    void Start()
    {
        cineCamera = GetComponent<CinemachineVirtualCamera>();
    }


    /// <summary>
    /// Call this function to shake the camera
    /// </summary>
    /// <param name="intesity">How intense the screen shake</param>
    /// <param name="time"> How long the screen shake is</param>
    public void ShakeCamera(float intesity, float time)
    {
        var cinemachineBasicMultiChannelPerlin =
            cineCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intesity;
        shakeTimer = time;
    }


    private void Update()
    {
        if (!(shakeTimer > 0)) return;
        shakeTimer -= Time.deltaTime;

        if (!(shakeTimer <= 0)) return;
        var cinemachineBasicMultiChannelPerlin =
            cineCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
    }
}