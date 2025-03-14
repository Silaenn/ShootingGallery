using UnityEngine;

public class GunShoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        GameObject bullet = Instantiate(bulletPrefab, mousePosition, Quaternion.identity);
    }
}
