using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    [SerializeField] PauseScene pauseScene;
    RectTransform crosshairRect;

    void Start()
    {
        if (pauseScene == null)
        {
            pauseScene = FindObjectOfType<PauseScene>();
            if (pauseScene == null)
            {
                Debug.LogError("PauseScene tidak ditemukan di scene!");
            }
        }

        crosshairRect = GetComponent<RectTransform>();
        if (crosshairRect == null)
        {
            Debug.LogError("CrosshairController membutuhkan RectTransform untuk UI!");
        }

        UpdateCursorVisibility();
    }

    void Update()
    {
        if (pauseScene.IsPaused())
        {
            Cursor.visible = true;
            return;
        }

        UpdateCrosshairPosition();
        Cursor.visible = false;
    }

    void UpdateCrosshairPosition()
    {
        if (crosshairRect != null)
        {
            Vector3 mousePosition = Input.mousePosition;
            transform.position = mousePosition;
        }
    }

    void UpdateCursorVisibility()
    {
        Cursor.visible = pauseScene != null && pauseScene.IsPaused();
    }

    public void OnResume()
    {
        UpdateCursorVisibility();
    }
}