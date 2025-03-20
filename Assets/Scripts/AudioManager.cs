using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    AudioSource audioSource;

    AudioSource bgmSource;

    public AudioClip soundReload;
    public AudioClip soundShoot;
    public AudioClip bgmClip;
    public AudioClip soundClick;

    bool isSoundMuted = false;
    bool isMusicMuted = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("AudioManager initialized and set to DontDestroyOnLoad.");
        }
        else
        {
            Debug.LogWarning("Duplicate AudioManager found. Destroying duplicate.");
            Destroy(gameObject);
        }

        InitializeAudioSources();
    }

    void InitializeAudioSources()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.volume = 0.644f;
        bgmSource.loop = true;

        if (bgmClip != null)
        {
            PlayBGM();
        }
    }

    public void ToggleSoundMute()
    {
        isSoundMuted = !isSoundMuted;
        audioSource.mute = isSoundMuted;
    }
    public void ToggleMusicMute()
    {
        isMusicMuted = !isMusicMuted;
        bgmSource.mute = isMusicMuted;
    }
    public bool IsSoundMuted()
    {
        return isSoundMuted;
    }
    public bool IsMusicMuted()
    {
        return isMusicMuted;
    }
    public void ReloadAudio()
    {
        if (!isSoundMuted)
        {
            audioSource.PlayOneShot(soundReload);
        }
    }

    public void ShootAudio()
    {
        if (!isSoundMuted)
        {
            audioSource.PlayOneShot(soundShoot);
        }
    }
    public void ClickAudio()
    {
        if (!isSoundMuted)
        {
            audioSource.PlayOneShot(soundClick);
        }
    }

    public void PlayBGM()
    {
        if (bgmClip != null)
        {
            bgmSource.clip = bgmClip;
            if (!isMusicMuted)
            {
                bgmSource.Play();
            }
        }
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }
}