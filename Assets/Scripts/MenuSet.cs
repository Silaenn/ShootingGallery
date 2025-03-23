using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSet : MonoBehaviour
{
    [SerializeField] PauseScene pauseScene;
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
    }
    public void MainMenu()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ClickAudio();
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
        if (pauseScene != null && pauseScene.IsPaused())
        {
            pauseScene.RestartGame();
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
