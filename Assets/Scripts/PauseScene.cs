using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseScene : MonoBehaviour
{
    [SerializeField] GameObject panelPause;
    [SerializeField] Sprite spritePause;
    [SerializeField] Sprite spritePlay;
    [SerializeField] Button pauseButton; // Tombol UI untuk pause

    Image buttonImage; // Komponen Image pada tombol
    bool isPaused = false;
    CrosshairController crosshairController;

    void Start()
    {
        // Ambil komponen Image dari tombol
        buttonImage = pauseButton.GetComponent<Image>();
        crosshairController = FindObjectOfType<CrosshairController>();
        panelPause.SetActive(false);

        // Tambahkan listener ke tombol untuk memanggil TogglePause
        pauseButton.onClick.AddListener(TogglePause);
    }

    void TogglePause()
    {
        if (!isPaused)
        {
            AudioManager.Instance.ClickAudio();
            Time.timeScale = 0;
            panelPause.SetActive(true);
            if (buttonImage != null && spritePause != null)
            {
                buttonImage.sprite = spritePause;
            }
            isPaused = true;
        }
        else
        {
            ResumeGame();
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
        if (buttonImage != null && spritePlay != null)
        {
            buttonImage.sprite = spritePlay;
        }
        panelPause.SetActive(false);
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