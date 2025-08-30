using UnityEngine;

public class UIGunMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // Kecepatan pergerakan senjata
    [SerializeField] private float maxOffset = 200f; // Batas offset horizontal (dalam pixel)
    [SerializeField] private Canvas canvas; // Referensi ke Canvas
    private RectTransform gunRectTransform;
    private Vector2 initialPosition;

    void Start()
    {
        // Ambil RectTransform dari UI Image
        gunRectTransform = GetComponent<RectTransform>();
        if (gunRectTransform == null)
        {
            Debug.LogError("RectTransform tidak ditemukan pada GameObject ini!");
            enabled = false;
            return;
        }

        // Cari Canvas jika tidak diatur di Inspector
        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("Canvas tidak ditemukan!");
                enabled = false;
                return;
            }
        }

        // Simpan posisi awal senjata
        initialPosition = gunRectTransform.anchoredPosition;
        Debug.Log($"UIGunMovement diinisialisasi, initialPosition: {initialPosition}");
    }

    void Update()
    {
        if (!enabled)
        {
            Debug.Log("UIGunMovement dinonaktifkan");
            return;
        }

        // Konversi posisi mouse ke koordinat lokal Canvas
        Vector2 mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.worldCamera,
            out mousePosition
        );

        // Hitung offset horizontal berdasarkan posisi mouse
        float offsetX = mousePosition.x - initialPosition.x;
        offsetX = Mathf.Clamp(offsetX, -maxOffset, maxOffset);

        // Tentukan posisi target
        Vector2 targetPosition = new Vector2(initialPosition.x + offsetX, gunRectTransform.anchoredPosition.y);

        // Lerp ke posisi target untuk pergerakan halus
        gunRectTransform.anchoredPosition = Vector2.Lerp(
            gunRectTransform.anchoredPosition,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        // Debug untuk melacak pergerakan
        Debug.Log($"MousePosition: {mousePosition}, TargetPosition: {targetPosition}, CurrentPosition: {gunRectTransform.anchoredPosition}");
    }

    public void ResetPosition()
    {
        initialPosition = gunRectTransform.anchoredPosition;
        Debug.Log($"ResetPosition dipanggil, initialPosition: {initialPosition}");
    }
}