using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] Sprite newChildSprite;
    [SerializeField] GameObject childObject;
    [SerializeField] GameObject textGo;

    [Header("Rotation Settings")]
    [SerializeField] float lerpDuration = 0.5f;
    [SerializeField] float rotationAngle = 180f;

    [Header("Scene Settings")]
    [SerializeField] string tutorialSceneName = "MainTutorial";
    [SerializeField] string gameSceneName = "MainGame";

    SpriteRenderer childSpriteRenderer;

    void Start()
    {
        if (childObject != null)
        {
            childSpriteRenderer = childObject.GetComponent<SpriteRenderer>();
        }
        else
        {
            childSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (childSpriteRenderer == null)
        {
            Debug.LogWarning("Child SpriteRenderer tidak ditemukan pada " + gameObject.name);
        }
    }
    void OnMouseDown()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ClickAudio();
        }

        if (childSpriteRenderer != null && newChildSprite != null)
        {
            childSpriteRenderer.sprite = newChildSprite;
            if (textGo != null)
            {
                textGo.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("Child SpriteRenderer atau newChildSprite tidak di-assign!");
        }

        Transform targetTransform = transform;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogError("Gagal menambahkan Rigidbody2D ke " + gameObject.name);
                return;
            }
        }

        Quaternion startRotation = targetTransform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, rotationAngle, 0);

        if (rb != null)
        {
            StartCoroutine(PhysicsUtils.RotateAndEnableGravity(targetTransform, rb, startRotation, targetRotation, lerpDuration));
        }

        StartCoroutine(LoadNextSceneAfterDelay(lerpDuration));
    }

    IEnumerator LoadNextSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (PlayerPrefs.GetInt("HasCompletedTutorial", 0) == 0)
        {
            SceneManager.LoadScene(tutorialSceneName);
        }
        else
        {
            SceneManager.LoadScene(gameSceneName);
        }
    }

    public void BackToMenu()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ClickAudio();
        }
        SceneManager.LoadScene("MainMenu");
    }

}
