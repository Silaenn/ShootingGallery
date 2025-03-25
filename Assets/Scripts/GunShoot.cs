using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class GunShoot : MonoBehaviour
{
    [SerializeField] Sprite newSprite;
    [SerializeField] AmmoManager ammoManager;
    [SerializeField] GameObject bulletMarks;
    [SerializeField] Camera mainCamera;

    [SerializeField] PauseScene pauseScene;
    [SerializeField] float bulletMarkLifetime = 3f;
    [SerializeField] float targetDestroyDelay = 3f;
    [SerializeField] LayerMask targetLayerMask;

    [Header("Score Text Settings")]
    [SerializeField] GameObject scoreTextPrefab;
    [SerializeField] float textOffsetX = 0.5f;
    [SerializeField] float textOffsetY = 0.5f;
    [SerializeField] float textDuration = 6f;

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera tidak ditemukan!");
            enabled = false;
            return;
        }
        if (pauseScene == null) pauseScene = FindAnyObjectByType<PauseScene>();
        if (pauseScene == null) Debug.LogWarning("PauseScene tidak ditemukan!");

        Cursor.visible = false;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
        {
            Shoot();
        }
    }

    bool IsPointerOverUI()
    {
        if (EventSystem.current == null)
        {
            Debug.LogError("EventSystem tidak ada di scene!");
            return false;
        }

        PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponent<Button>() != null)
            {
                Debug.Log("Klik pada tombol UI terdeteksi, tidak menembak");
                return true;
            }
        }

        return false;
    }

    void Shoot()
    {
        if (pauseScene != null && pauseScene.IsPaused()) return;
        if (!ammoManager.UseAmmo()) return;

        AudioManager.Instance.ShootAudio();

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, targetLayerMask);

        if (hit.collider != null && hit.collider.CompareTag("Target"))
        {
            HandleTargetHit(hit);
        }
        else
        {
            SpawnBulletMark(ray);
        }
    }

    void HandleTargetHit(RaycastHit2D hit)
    {
        MovingTarget target = hit.collider.GetComponent<MovingTarget>();
        if (target != null)
        {
            SurvivalTimer timer = FindAnyObjectByType<SurvivalTimer>();
            if (timer != null)
            {
                timer.AddScore(target.scoreValue);
                SpawnScoreText(hit.point, target.scoreValue);
            }
        }

        SpriteRenderer targetSpriteRenderer = hit.collider.GetComponent<SpriteRenderer>();
        if (targetSpriteRenderer != null && newSprite != null) targetSpriteRenderer.sprite = newSprite;

        Transform targetTransform = hit.collider.transform;
        Rigidbody2D targetRb = hit.collider.GetComponent<Rigidbody2D>();
        if (targetRb == null)
        {
            Debug.LogError("Target " + hit.collider.gameObject.name + " tidak punya Rigidbody2D!");
            return;
        }

        Collider2D targetCollider = hit.collider.GetComponent<Collider2D>();
        if (targetCollider != null) targetCollider.enabled = false;

        float lerpDuration = 0.5f;
        Quaternion startRotation = targetTransform.rotation;
        float targetRotationAngle = targetTransform.localScale.x > 0 ? 180f : -180f;
        Quaternion targetRotation = Quaternion.Euler(0, targetRotationAngle, 0);

        targetRb.isKinematic = false;
        StartCoroutine(PhysicsUtils.RotateAndEnableGravity(targetTransform, targetRb, startRotation, targetRotation, lerpDuration));

        if (targetTransform.GetComponent<MovingTarget>() != null)
        {
            targetTransform.GetComponent<MovingTarget>().enabled = false;
        }

        Destroy(hit.collider.gameObject, targetDestroyDelay);
    }

    void SpawnBulletMark(Ray ray)
    {
        Vector3 bulletMarkPosition = ray.origin + (ray.direction * 10f);
        GameObject bulletMarkInstance = Instantiate(bulletMarks, bulletMarkPosition, Quaternion.identity);
        Destroy(bulletMarkInstance, bulletMarkLifetime);
    }

    void SpawnScoreText(Vector2 position, int score)
    {
        if (scoreTextPrefab != null)
        {
            Vector3 spawnPosition = new Vector3(position.x + textOffsetX, position.y + textOffsetY, 0);
            GameObject scoreTextObj = Instantiate(scoreTextPrefab, spawnPosition, Quaternion.identity);

            MeshRenderer textRenderer = scoreTextObj.GetComponent<MeshRenderer>();
            if (textRenderer != null)
            {
                textRenderer.sortingLayerName = "Default";
                textRenderer.sortingOrder = 40;
            }
            else
            {
                Debug.LogError("MeshRenderer tidak ditemukan di scoreTextObj!");
            }

            TextMeshPro scoreText = scoreTextObj.GetComponent<TextMeshPro>();
            if (scoreText != null)
            {
                scoreText.text = "+" + score;
            }
            else
            {
                Debug.LogError("TextMeshPro component tidak ditemukan di scoreTextPrefab!");
            }

            StartCoroutine(AnimateScoreText(scoreTextObj));
        }
        else
        {
            Debug.LogWarning("ScoreTextPrefab tidak diatur di Inspector!");
        }
    }

    IEnumerator AnimateScoreText(GameObject textObj)
    {
        float elapsed = 0f;
        Vector3 startPos = textObj.transform.position;

        while (elapsed < textDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / textDuration;
            textObj.transform.position = startPos + new Vector3(0, t, 0);
            yield return null;
        }

        Destroy(textObj);
    }

}
