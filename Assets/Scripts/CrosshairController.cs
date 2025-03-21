using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    private PauseScene pauseScene;

    void Start()
    {
        pauseScene = FindObjectOfType<PauseScene>();
        if (pauseScene == null)
        {
            Debug.LogError("PauseScene tidak ditemukan di scene!");
        }
    }

    void Update()
    {
        if (pauseScene != null && pauseScene.IsPaused())
        {
            enabled = false;
            return;
        }

        Vector3 mousePosition = Input.mousePosition;
        transform.position = mousePosition;
    }

    public void ResumeCrosshair()
    {
        enabled = true;
    }
}