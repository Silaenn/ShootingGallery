using UnityEngine;
using System.Collections;

public static class PhysicsUtils
{
    public static IEnumerator RotateAndEnableGravity(Transform targetTransform, Rigidbody2D rb, Quaternion startRotation, Quaternion targetRotation, float lerpDuration)
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