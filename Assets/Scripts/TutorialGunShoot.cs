using UnityEngine;

public class TutorialGunShoot : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] AmmoManager ammoManager;
    [SerializeField] LayerMask targetLayerMask;

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
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
        if (!ammoManager.UseAmmo()) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, targetLayerMask);

        if (hit.collider != null && hit.collider.CompareTag("Target"))
        {
            Destroy(hit.collider.gameObject);
        }
    }
}