using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Target"))
        {
            Debug.Log("Masuk");
            Destroy(other.gameObject, 2f);
            Destroy(gameObject, 2f);
        }
    }
}
