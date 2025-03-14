using UnityEngine;

public class GunMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float maxOffset = 2f;

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z; // Tetap di sumbu Z yang sama

        float offsetX = mousePosition.x - initialPosition.x;

        offsetX = Mathf.Clamp(offsetX, -maxOffset, maxOffset);

        Vector3 targetPosition = new Vector3(initialPosition.x + offsetX, transform.position.y, transform.position.z);

        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }
}
