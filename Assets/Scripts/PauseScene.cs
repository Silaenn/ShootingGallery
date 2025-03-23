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
        crosshairController = FindObjectOfType<CrosshairController>();
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
        if (crosshairController != null)
        {
            crosshairController.OnResume();
        }
        isPaused = false;
    }

    public void RestartGame()
    {
        Debug.Log("Masuk MainGame");
        AudioManager.Instance.ClickAudio();
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}