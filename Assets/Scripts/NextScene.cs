using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    void OnMouseDown()
    {
        AudioManager.Instance.ClickAudio();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}
