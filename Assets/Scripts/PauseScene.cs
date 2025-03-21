using UnityEngine;

public class PauseScene : MonoBehaviour
{
    [SerializeField] GameObject panelPause;
    [SerializeField] Sprite spritePause;

    private SpriteRenderer spriteRenderer;
    private bool isPaused = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        Time.timeScale = 1;
        panelPause.SetActive(false);
        isPaused = false;
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}