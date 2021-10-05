using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set;}

    public Sound[] sounds;

    private string currentlyPlaying;

    private float minPitch = 0.7f;
    private float maxPitch = 1.6f;

    private Coroutine pitchCoroutine;

    private WaitForSecondsRealtime tick = new WaitForSecondsRealtime(0.1f);

    private float CurrPitch => FindSound(GetCurrentlyPlaying()).source.pitch;
    private void Awake()
    {
        Instance = this;

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

            if (s.spatialBlend3D)
                s.source.spatialBlend = 1f;
        }
    }
    private void Update()
    {
        if (Instance == null)
            Instance = this;
    }
    public Sound FindSound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name.Equals(name));

        if (s == null)
            Debug.LogWarning("Sound: " + name + " was not found. Please check typo :)");

        return s;
    }
    public void Play (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name.Equals(name));

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " was not found. Please check typo :)");
            return;
        }

        if (name.Equals(GetCurrentlyPlaying()))
            return;

        if (!name.Equals(GetCurrentlyPlaying()) && s.isMusic && GetCurrentlyPlaying() != null)
        {
            Mute(GetCurrentlyPlaying());
        }

        if (s.isMusic)
            currentlyPlaying = name;

        if (s.isCommonSFX)
        {
            s.source.pitch = new RandomFloat().NextFloat(minPitch, maxPitch);
        }

        s.source.Play();
    }
    public void Play (string name, Vector3 position)
    {
        Sound s = Array.Find(sounds, sound => sound.name.Equals(name));

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " was not found. Please check typo :)");
            return;
        }

        if (name.Equals(GetCurrentlyPlaying()))
            return;

        if (!name.Equals(GetCurrentlyPlaying()) && s.isMusic && GetCurrentlyPlaying() != null)
        {
            Mute(GetCurrentlyPlaying());
        }

        if (s.isMusic)
            currentlyPlaying = name;

        if (s.isCommonSFX)
        {
            s.source.pitch = new RandomFloat().NextFloat(minPitch, maxPitch);
        }

        if (s.spatialBlend3D)
            AudioManager.Instance.transform.position = position;

        s.source.Play();
    }
    public void Mute (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name.Equals(name));

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " was not found. Please check typo :)");
            return;
        }

        s.source.Stop();
    }
    public string GetCurrentlyPlaying()
    {
        return currentlyPlaying;
    }
    public void ChangePitch(float target, float duration)
    {
        if (pitchCoroutine != null)
            StopCoroutine(pitchCoroutine);

        if (duration <= Mathf.Epsilon && duration > 0)
            SetPitch(target);

        pitchCoroutine = StartCoroutine(ChangePitchCo(target, duration));
    }
    private IEnumerator ChangePitchCo(float target, float duration)
    {
        float from = CurrPitch;
        float invDuration = 1.0f / duration;

        float progress = Time.unscaledDeltaTime * invDuration;

        while(Mathf.Abs(CurrPitch - target) > 0.0f)
        {
            FindSound(GetCurrentlyPlaying()).source.pitch = Mathf.Lerp(from, target, progress);
            progress += Time.unscaledDeltaTime * invDuration;
            yield return null;
        }

        //if (s.source.pitch >= p)
        //{
        //    while (s.source.pitch > p)
        //    {
        //        s.source.pitch -= pitchChangePerTick;
        //        yield return tick;
        //    }
        //    if (s.source.pitch < p)
        //        s.source.pitch = p;
        //}
        //else if (s.source.pitch <= p)
        //{
        //    while (s.source.pitch < p)
        //    {
        //        s.source.pitch += pitchChangePerTick;
        //        yield return tick;
        //    }
        //    if (s.source.pitch > p)
        //        s.source.pitch = p;
        //}
    }
    private void SetPitch(float pitch)
    {
        if (pitchCoroutine != null)
            StopCoroutine(pitchCoroutine);

        FindSound(GetCurrentlyPlaying()).source.pitch = pitch;
    }
}
