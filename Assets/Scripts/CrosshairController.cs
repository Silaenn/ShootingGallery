using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;

        transform.position = mousePosition;
    }
}
