using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class GunShoot : MonoBehaviour
{
    public Sprite newSprite;
    public AmmoManager ammoManager;
    public GameObject bulletMarks;
    Camera mainCamera;
    float timeDestroy = 3f;
    PauseScene pauseScene;

    void Start()
    {
        mainCamera = Camera.main;
        Cursor.visible = false;
        pauseScene = FindAnyObjectByType<PauseScene>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current == null)
            {
                Debug.LogError("EventSystem tidak ada di scene!");
                return;
            }

            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (RaycastResult result in results)
            {
                Debug.Log("UI Raycast hit: " + result.gameObject.name);

                if (result.gameObject.GetComponent<Button>() != null)
                {
                    Debug.Log("Klik pada tombol UI terdeteksi, tidak menembak");
                    return;
                }
            }

            Shoot();
        }
    }

    void Shoot()
    {
        if (pauseScene != null && pauseScene.IsPaused())
        {
            return; // Blokir tembakan saat pause
        }

        if (!ammoManager.UseAmmo())
        {
            return; // Blokir jika ammo habis
        }

        int layerMask = ~0;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, layerMask);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Target"))
            {
                AudioManager.Instance.ShootAudio();
                MovingTarget target = hit.collider.GetComponent<MovingTarget>();
                if (target != null)
                {
                    int targetScore = target.scoreValue;
                    SurvivalTimer timer = FindAnyObjectByType<SurvivalTimer>();
                    if (timer != null)
                    {
                        timer.AddScore(targetScore);
                    }
                }
                SpriteRenderer targetSpriteRenderer = hit.collider.GetComponent<SpriteRenderer>();

                Transform targetTransform = hit.collider.transform;
                Rigidbody2D targetRb = hit.collider.GetComponent<Rigidbody2D>();

                if (targetRb == null)
                {
                    targetRb = hit.collider.gameObject.AddComponent<Rigidbody2D>();
                }

                Collider2D targetCollider = hit.collider.GetComponent<Collider2D>();
                if (targetCollider != null)
                {
                    targetCollider.enabled = false;
                }

                if (targetSpriteRenderer != null)
                {
                    targetSpriteRenderer.sprite = newSprite;
                }

                float lerpDuration = 0.5f;
                Quaternion startRotation = targetTransform.rotation;
                float targetRotationAngle = targetTransform.localScale.x > 0 ? 180f : -180f;
                Quaternion targetRotation = Quaternion.Euler(0, targetRotationAngle, 0);

                StartCoroutine(PhysicsUtils.RotateAndEnableGravity(targetTransform, targetRb, startRotation, targetRotation, lerpDuration));

                targetTransform.GetComponent<MovingTarget>().enabled = false;

                Destroy(hit.collider.gameObject, timeDestroy);
            }
        }
        else
        {
            AudioManager.Instance.ShootAudio();
            Vector3 bulletMarkPosition = ray.origin + (ray.direction * 10f);
            GameObject bulletMarkInstance = Instantiate(bulletMarks, bulletMarkPosition, Quaternion.identity);
            Destroy(bulletMarkInstance, 3f);
            Debug.Log("Raycast tidak kena apa-apa!");
        }
    }
}
