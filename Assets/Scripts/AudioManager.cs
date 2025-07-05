using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource audioSource;
    public AudioSource bgmSource;

    [Header("Audio Clips")]
    public AudioClip soundReload;
    public AudioClip soundShoot;
    public AudioClip soundClick;
    public AudioClip bgmClip;

    [Header("Settings")]
    [SerializeField][Range(0f, 1f)] float bgmVolume = 0.644f;

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
        LoadMuteSettings();
        SetupBGM();
    }

    void InitializeAudioSources()
    {
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.volume = bgmVolume;
        }
    }

    void SetupBGM()
    {
        if (bgmClip != null)
        {
            bgmSource.clip = bgmClip;
            if (!isMusicMuted && !bgmSource.isPlaying) bgmSource.Play();
        }
    }

    void LoadMuteSettings()
    {
        isMusicMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
        isSoundMuted = PlayerPrefs.GetInt("SoundMuted", 0) == 1;
        bgmSource.mute = isMusicMuted;
        audioSource.mute = isSoundMuted;
    }

    public void ToggleMute(bool isMusic)
    {
        if (isMusic)
        {
            isMusicMuted = !isMusicMuted;
            PlayerPrefs.SetInt("MusicMuted", isMusicMuted ? 1 : 0);
            if (bgmSource != null)
            {
                bgmSource.mute = isMusicMuted;
                if (!isMusicMuted && !bgmSource.isPlaying)
                {
                    bgmSource.Play();
                }
            }
        }
        else
        {
            isSoundMuted = !isSoundMuted;
            PlayerPrefs.SetInt("SoundMuted", isSoundMuted ? 1 : 0);
            audioSource.mute = isSoundMuted;
        }
        PlayerPrefs.Save();
    }
    public bool IsSoundMuted() => isSoundMuted;
    public bool IsMusicMuted() => isMusicMuted;

    public void ReloadAudio()
    {
        PlaySFX(soundReload);
    }

    public void ShootAudio()
    {
        PlaySFX(soundShoot);
    }
    public void ClickAudio()
    {
        PlaySFX(soundClick);
    }

    void PlaySFX(AudioClip clip)
    {
        if (!isSoundMuted && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void PlayBGM()
    {
        if (bgmClip != null && !bgmSource.isPlaying)
        {
            if (!isMusicMuted)
            {
                bgmSource.Play();
            }
        }
    }

    public void StopBGM()
    {
        if (bgmSource != null) bgmSource.Stop();
    }
}