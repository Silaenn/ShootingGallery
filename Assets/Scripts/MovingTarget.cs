using System;
using System.Collections;
using UnityEngine;

public class MovingTarget : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public int scoreValue = 20;
    [SerializeField] float amplitude = 0.6f;
    [SerializeField] float frequency = 2f;
    [SerializeField] float destroyDistance = 18f;
    [SerializeField] bool startFacingRight = true;

    Vector2 startPosition;
    SpriteRenderer spriteRenderer;
    float direction;

    void Start()
    {
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer tidak ditemukan pada " + gameObject.name);
            enabled = false;
            return;
        }

        direction = startFacingRight ? 1 : -1;
        FlipSprite();
    }

    void Update()
    {
        MoveHorizontal();
        WaveMovement();
    }

    void WaveMovement()
    {
        float sinWave = Mathf.Sin(Time.time * frequency);
        float verticalOffset = Mathf.Abs(sinWave) * amplitude;
        transform.position = new Vector2(
            transform.position.x,
            startPosition.y + verticalOffset
        );

        float horizontalDistance = Mathf.Abs(transform.position.x - startPosition.x);
        if (horizontalDistance >= destroyDistance)
        {
            Destroy(gameObject);
        }
    }

    void MoveHorizontal()
    {
        transform.Translate(Vector2.right * direction * moveSpeed * Time.deltaTime);
    }

    void FlipSprite()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = (direction == -1);
        }
    }

}
