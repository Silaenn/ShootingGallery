using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScene : MonoBehaviour
{
    [SerializeField] GameObject panelPause;
    [SerializeField] Sprite spritePause;

    SpriteRenderer spriteRenderer;
    bool isPaused = false;
    CrosshairController crosshairController;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        crosshairController = FindAnyObjectByType<CrosshairController>();
        panelPause.SetActive(false);
    }

    void OnMouseDown()
    {
        if (!isPaused)
        {
            AudioManager.Instance.ClickAudio();
            Time.timeScale = 0;
            panelPause.SetActive(true);
            spriteRenderer.sprite = spritePause;
            isPaused = true;
        }
    }

    public void ResumeGame()
    {
        AudioManager.Instance.ClickAudio();
        Time.timeScale = 1;
        crosshairController.ResumeCrosshair();
        panelPause.SetActive(false);
        isPaused = false;
    }

    public void RestartGame()
    {
        AudioManager.Instance.ClickAudio();
        Time.timeScale = 1;
        panelPause.SetActive(false);
        SceneManager.LoadScene("MainGame");
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}