using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundToggleButton : MonoBehaviour
{
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;

    SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = AudioManager.Instance.IsSoundMuted() ? soundOffSprite : soundOnSprite;
    }

    void OnMouseDown()
    {
        AudioManager.Instance.ToggleSoundMute();
        spriteRenderer.sprite = AudioManager.Instance.IsSoundMuted() ? soundOffSprite : soundOnSprite;

        if (!AudioManager.Instance.IsSoundMuted())
        {
            AudioManager.Instance.ClickAudio();
        }
    }
}
