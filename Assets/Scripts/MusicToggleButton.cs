using UnityEngine;
using UnityEngine.UI;

public class MusicToggleButton : MonoBehaviour
{
    [SerializeField] Sprite musicOnSprite;
    [SerializeField] Sprite musicOffSprite;

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
        AudioManager.Instance.ToggleMute(true);
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
            spriteRenderer.sprite = AudioManager.Instance.IsMusicMuted() ? musicOffSprite : musicOnSprite;
        }
    }

}
