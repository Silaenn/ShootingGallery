using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class SoundToggleButton : MonoBehaviour
{
    [SerializeField] Sprite soundOnSprite;
    [SerializeField] Sprite soundOffSprite;

    SpriteRenderer spriteRenderer;

    void Start()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogError("AudioManager.Instance tidak ditemukan! Pastikan AudioManager ada di scene.");
            return;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSprite();
    }

    void OnMouseDown()
    {
        AudioManager.Instance.ToggleMute(false);
        UpdateSprite();

        if (!AudioManager.Instance.IsSoundMuted())
        {
            AudioManager.Instance.ClickAudio();
        }
    }

    void UpdateSprite()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = AudioManager.Instance.IsSoundMuted() ? soundOffSprite : soundOnSprite;
        }
    }

}
