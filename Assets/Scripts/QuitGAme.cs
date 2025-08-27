using UnityEngine;

public class QuitGame : MonoBehaviour
{
    void OnMouseDown()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetTutorial();
        }
#endif
    }
}
