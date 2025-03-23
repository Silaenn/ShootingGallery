using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSet : MonoBehaviour
{
    PauseScene pauseScene;

    void Start()
    {
        pauseScene = FindAnyObjectByType<PauseScene>();
    }
    public void MainMenu()
    {
        Debug.Log("Masuk MainMenu");
        AudioManager.Instance.ClickAudio();
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void ReseumeIfPause()
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
        AudioManager.Instance.ClickAudio();
        Application.Quit();
    }




}
