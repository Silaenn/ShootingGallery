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
            return;
        }
    }

    void Start()
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

    public void ReloadAudio()
    {
        audioSource.PlayOneShot(soundReload);
    }

    public void ShootAudio()
    {
        audioSource.PlayOneShot(soundShoot);
    }

    public void PlayBGM()
    {
        if (bgmClip != null)
        {
            bgmSource.clip = bgmClip;
            bgmSource.Play();
        }
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }
}