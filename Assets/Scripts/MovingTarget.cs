using System;
using System.Collections;
using UnityEngine;

public class MovingTarget : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float amplitude = 0.5f;
    public float frequency = 1f;
    public float destroyDistance = 18f;
    public bool startFacingRight = true;

    Vector2 startPosition;
    SpriteRenderer spriteRenderer;
    float direction;

    void Start()
    {
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        float verticalOffset = Mathf.Sin(Time.time * frequency) * amplitude;
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
        spriteRenderer.flipX = (direction == -1);
    }

}
