using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    public Sprite newChildSprite;
    public GameObject childObject;
    public GameObject textGo;

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
    }
    void OnMouseDown()
    {
        AudioManager.Instance.ClickAudio();

        if (childSpriteRenderer != null && newChildSprite != null)
        {
            childSpriteRenderer.sprite = newChildSprite;
            textGo.SetActive(false);
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
        }

        Quaternion startRotation = targetTransform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, 180f, 0);
        float lerpDuration = 0.5f;

        StartCoroutine(PhysicsUtils.RotateAndEnableGravity(targetTransform, rb, startRotation, targetRotation, lerpDuration));

        StartCoroutine(LoadNextSceneAfterDelay(lerpDuration));
    }

    IEnumerator LoadNextSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}
