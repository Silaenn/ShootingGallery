using UnityEngine;

public class GunMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float maxOffset = 2f;
    [SerializeField] Camera mainCamera;

    private Vector3 initialPosition;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("Main Camera tidak ditemukan!");
                enabled = false;
                return;
            }
        }
        initialPosition = transform.position;
    }

    void Update()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z;

        float offsetX = mousePosition.x - initialPosition.x;

        offsetX = Mathf.Clamp(offsetX, -maxOffset, maxOffset);

        Vector3 targetPosition = new Vector3(initialPosition.x + offsetX, transform.position.y, transform.position.z);

        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    public void ResetPosition()
    {
        initialPosition = transform.position;
    }
}
