using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicToggleButton : MonoBehaviour
{
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;

    SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = AudioManager.Instance.IsMusicMuted() ? musicOffSprite : musicOnSprite;
    }

    void OnMouseDown()
    {
        AudioManager.Instance.ToggleMusicMute();
        spriteRenderer.sprite = AudioManager.Instance.IsMusicMuted() ? musicOffSprite : musicOnSprite;

        if (!AudioManager.Instance.IsSoundMuted())
        {
            AudioManager.Instance.ClickAudio();
        }
    }
}
