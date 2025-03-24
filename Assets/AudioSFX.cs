using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSFX : MonoBehaviour
{
    AudioSource audioSource;

    public AudioClip soundReadyGo;

    void Awake()
    {
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlaySoundReadyGo()
    {
        audioSource.PlayOneShot(soundReadyGo);
    }

}
