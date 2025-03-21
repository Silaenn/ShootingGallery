using UnityEngine;
using UnityEngine.UI;

public class MusicToggleButton : MonoBehaviour
{
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = AudioManager.Instance.IsMusicMuted() ? musicOffSprite : musicOnSprite;
        }
    }

    void OnMouseDown()
    {
        AudioManager.Instance.ToggleMusicMute();

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = AudioManager.Instance.IsMusicMuted() ? musicOffSprite : musicOnSprite;
        }


        if (!AudioManager.Instance.IsSoundMuted())
        {
            AudioManager.Instance.ClickAudio();
        }
    }
}
