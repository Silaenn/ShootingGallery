using UnityEngine;

public class GunShoot : MonoBehaviour
{
    public GameObject bulletHolePrefab;
    Camera mainCamera;

    float timeDestroy = 10f;

    void Start()
    {
        mainCamera = Camera.main;
        Cursor.visible = false;
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
        Debug.Log("Masuk");

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

        if (hit.collider != null)
        {
            Debug.Log("Kena: " + hit.collider.gameObject.name);
            GameObject bulletHole = Instantiate(bulletHolePrefab, hit.point, Quaternion.identity);

            if (hit.collider.CompareTag("Target"))
            {
                Destroy(hit.collider.gameObject, timeDestroy);
                Destroy(bulletHole, timeDestroy);
            }
        }
        else
        {
            Debug.Log("Raycast tidak kena apa-apa!");
        }
    }

}
