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
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeAudioSources();

        isMusicMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
        isSoundMuted = PlayerPrefs.GetInt("SoundMuted", 0) == 1;
        bgmSource.mute = isMusicMuted;
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
        PlayerPrefs.SetInt("SoundMuted", isSoundMuted ? 1 : 0);
        PlayerPrefs.Save();
        audioSource.mute = isSoundMuted;
    }
    public void ToggleMusicMute()
    {
        isMusicMuted = !isMusicMuted;
        PlayerPrefs.SetInt("MusicMuted", isMusicMuted ? 1 : 0);
        PlayerPrefs.Save();
        bgmSource.mute = isMusicMuted;

    }
    public bool IsSoundMuted()
    {
        return PlayerPrefs.GetInt("SoundMuted", 0) == 1;
    }
    public bool IsMusicMuted()
    {
        int value = PlayerPrefs.GetInt("MusicMuted", 0);
        return value == 1;
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