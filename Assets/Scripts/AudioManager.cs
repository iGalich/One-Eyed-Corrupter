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
        }
    }
    private void Update()
    {
        if (Instance == null)
            Instance = this;
    }
    public Sound FindSound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
            Debug.LogWarning("Sound: " + name + " was not found. Please check typo :)");

        return s;
    }
    public void Play (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " was not found. Please check typo :)");
            return;
        }

        if (name != GetCurrentlyPlaying() && s.isMusic)
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
    public void Mute (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

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
    
}
