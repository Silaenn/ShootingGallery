using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSet : MonoBehaviour
{
    [SerializeField] PauseScene pauseScene;
    [SerializeField] SurvivalTimer survivalTimer;
    [SerializeField] string mainMenuSceneName = "MainMenu";

    void Start()
    {
        if (pauseScene == null)
        {
            pauseScene = FindAnyObjectByType<PauseScene>();
            if (pauseScene == null)
            {
                Debug.LogWarning("PauseScene tdiak ditemukan di scene!");
            }
        }
        if (survivalTimer == null)
        {
            survivalTimer = FindAnyObjectByType<SurvivalTimer>();
            if (survivalTimer == null)
            {
                Debug.LogWarning("SurvivalTimer tdiak ditemukan di scene!");
            }
        }
    }
    public void MainMenu()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ClickAudio();
            AudioManager.Instance.PlayBGM();
        }
        Time.timeScale = 1;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void ResumeIfPause()
    {
        if (pauseScene != null && pauseScene.IsPaused())
        {
            pauseScene.ResumeGame();
        }
    }

    public void RestartGame()
    {
        if ((pauseScene != null && pauseScene.IsPaused()) || (survivalTimer != null && survivalTimer.IsGameOver()))
        {
            pauseScene.RestartGame();
            AudioManager.Instance.PlayBGM();
        }
    }

    public void QuitGame()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ClickAudio();
        }

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); 
#endif
    }

}
