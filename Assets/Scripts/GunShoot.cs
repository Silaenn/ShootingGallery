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
    Camera mainCamera;

    float timeDestroy = 3f;

    void Start()
    {
        mainCamera = Camera.main;
        Cursor.visible = false;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Debug untuk melihat apa yang terjadi
            Debug.Log("Klik terdeteksi");

            // Cek apakah EventSystem ada
            if (EventSystem.current == null)
            {
                Debug.LogError("EventSystem tidak ada di scene!");
                return;
            }

            // Cek apakah klik mengenai UI dengan lebih detail
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            // Cek hasil raycast UI
            foreach (RaycastResult result in results)
            {
                Debug.Log("UI Raycast hit: " + result.gameObject.name);

                // Jika kita mengenai tombol, jangan menembak
                if (result.gameObject.GetComponent<Button>() != null)
                {
                    Debug.Log("Klik pada tombol UI terdeteksi, tidak menembak");
                    return;
                }
            }

            // Tidak ada tombol UI yang terkena, aman untuk menembak
            Debug.Log("Menembak...");
            Shoot();
        }
    }

    void Shoot()
    {
        if (!ammoManager.UseAmmo())
        {
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Target"))
            {
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

                StartCoroutine(RotateAndEnableGravity(targetTransform, targetRb, startRotation, targetRotation, lerpDuration));

                targetTransform.GetComponent<MovingTarget>().enabled = false;

                Destroy(hit.collider.gameObject, timeDestroy);
            }
        }
        else
        {
            Debug.Log("Raycast tidak kena apa-apa!");
        }
    }

    IEnumerator RotateAndEnableGravity(Transform targetTransform, Rigidbody2D rb, Quaternion startRotation, Quaternion targetRotation, float lerpDuration)
    {
        float timer = 0f;

        rb.gravityScale = 0f;

        while (timer < lerpDuration)
        {
            timer += Time.deltaTime;
            float t = timer / lerpDuration;
            targetTransform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return null;
        }

        targetTransform.rotation = targetRotation;

        rb.gravityScale = 1f;

        rb.velocity = new Vector2(0, -3f);
    }
}
