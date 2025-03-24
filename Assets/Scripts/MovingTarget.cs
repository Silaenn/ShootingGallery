using System;
using System.Collections;
using UnityEngine;

public class MovingTarget : MonoBehaviour
{
    [Header("Movement Settings")]
    public float baseSpeed = 1.5f;
    public int scoreValue = 20;
    [SerializeField] float speedIncreasePerDifficulty = 0.2f;
    [SerializeField] float maxSpeed = 3.5f;

    [SerializeField] float amplitude = 0.6f;
    [SerializeField] float frequency = 2f;
    [SerializeField] float destroyDistance = 18f;
    [SerializeField] bool startFacingRight = true;


    Vector2 startPosition;
    SpriteRenderer spriteRenderer;
    float direction;
    SurvivalTimer survivalTimer;
    float moveSpeed;

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

        survivalTimer = FindAnyObjectByType<SurvivalTimer>();
        if (survivalTimer == null)
        {
            Debug.LogError("SurvivalTimer tidak ditemukan di scene");
        }

        direction = startFacingRight ? 1 : -1;
        FlipSprite();

        UpdateMoveSpeed();
    }

    void Update()
    {
        UpdateMoveSpeed();
        MoveHorizontal();
        WaveMovement();
    }

    void UpdateMoveSpeed()
    {
        if (survivalTimer != null)
        {
            moveSpeed = Mathf.Min(baseSpeed + (speedIncreasePerDifficulty * (survivalTimer.GetDifficultyLevel() - 1)), maxSpeed);
        }
        else
        {
            moveSpeed = baseSpeed;
        }
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
