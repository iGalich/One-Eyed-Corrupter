using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineShake : MonoBehaviour
{
    public static CinemachineShake Insatnce { get; private set; }

    private CinemachineVirtualCamera vcam;
    private float shakeTimerTotal;
    private float shakeTimer;
    private float startingIntensity;

    private bool isShaking;

    [SerializeField] private float cameraShakeIntensity = 5f;

    public bool IsShaking { get => isShaking; set => isShaking = value; }

    private void Start()
    {
        Insatnce = this;
        vcam = GetComponent<CinemachineVirtualCamera>();
        isShaking = false;
    }
    public void ShakeCamera(float intensity,float time)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        startingIntensity = intensity;
        shakeTimerTotal = time;
        shakeTimer = time;
        isShaking = true;
    }
    private void Update()
    {
        if (shakeTimer > 0 && isShaking)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0f)
            {
                CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, 1 - (shakeTimer / shakeTimerTotal));
            }
        }
        isShaking = false;
        if (!isShaking)
        {
            ShakeCamera(0f, 0.1f);
        }
    }
    public float GetCameraShakeIntensity() { return cameraShakeIntensity; }
}
