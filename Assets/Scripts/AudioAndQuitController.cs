using UnityEngine;
using UnityEngine.UI;

public class AudioAndQuitController : MonoBehaviour
{
    [Header("Sound Toggle Settings")]
    [SerializeField] Button soundToggleButton;
    [SerializeField] Image soundToggleImage;
    [SerializeField] Sprite soundOnSprite;
    [SerializeField] Sprite soundOffSprite;

    [Header("Music Toggle Settings")]
    [SerializeField] Button musicToggleButton;
    [SerializeField] Image musicToggleImage;
    [SerializeField] Sprite musicOnSprite;
    [SerializeField] Sprite musicOffSprite;

    [Header("Quit Button")]
    [SerializeField] Button quitButton;

    void Start()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogError("AudioManager.Instance tidak ditemukan! Pastikan AudioManager ada di scene.");
            return;
        }

        // Setup button listeners
        if (soundToggleButton != null)
        {
            soundToggleButton.onClick.AddListener(ToggleSound);
        }
        
        if (musicToggleButton != null)
        {
            musicToggleButton.onClick.AddListener(ToggleMusic);
        }
        
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }

        UpdateSoundSprite();
        UpdateMusicSprite();
    }

    void ToggleSound()
    {
        AudioManager.Instance.ToggleMute(false);
        UpdateSoundSprite();

        if (!AudioManager.Instance.IsSoundMuted())
        {
            AudioManager.Instance.ClickAudio();
        }
    }

    void ToggleMusic()
    {
        AudioManager.Instance.ToggleMute(true);
        UpdateMusicSprite();

        if (!AudioManager.Instance.IsSoundMuted())
        {
            AudioManager.Instance.ClickAudio();
        }
    }

    void UpdateSoundSprite()
    {
        if (soundToggleImage != null)
        {
            soundToggleImage.sprite = AudioManager.Instance.IsSoundMuted() ? soundOffSprite : soundOnSprite;
        }
    }

    void UpdateMusicSprite()
    {
        if (musicToggleImage != null)
        {
            musicToggleImage.sprite = AudioManager.Instance.IsMusicMuted() ? musicOffSprite : musicOnSprite;
        }
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetTutorial();
        }
        Application.Quit();
#endif
    }
}