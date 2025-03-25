using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] GameObject overlayPanel;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] GameObject ammoIconsParent;
    [SerializeField] Button reloadButton;
    [SerializeField] GameObject highlightBox;
    [SerializeField] RectTransform handCursor;
    [SerializeField] GameObject target1;
    [SerializeField] GameObject target2;
    [SerializeField] TextMeshProUGUI endText;
    [SerializeField] GunShoot gunShoot;
    [SerializeField] AmmoManager ammoManager;

    [SerializeField] float stepDuration = 4f;
    private RectTransform highlightRect;

    void Start()
    {
        highlightRect = highlightBox.GetComponent<RectTransform>();
        overlayPanel.SetActive(true);
        highlightBox.SetActive(false);
        handCursor.gameObject.SetActive(false);
        target1.SetActive(false);
        target2.SetActive(false);
        endText.gameObject.SetActive(false);

        if (gunShoot != null) gunShoot.enabled = false;

        StartCoroutine(RunTutorial());
    }

    System.Collections.IEnumerator RunTutorial()
    {
        // Langkah 1: Waktu
        HighlightElement(timerText.gameObject, "Ini waktu permainan!");
        yield return new WaitForSecondsRealtime(stepDuration);

        // Langkah 2: Skor
        HighlightElement(scoreText.gameObject, "Ini skor kamu!");
        yield return new WaitForSecondsRealtime(stepDuration);

        target1.SetActive(true);
        if (gunShoot != null) gunShoot.enabled = true;
        HighlightElement(target1, "Tembak target ini 4 kali!");
        int shotsFired = 0;
        yield return new WaitUntil(() =>
        {
            if (Input.GetMouseButtonDown(0))
            {
                gunShoot.Shoot();
                shotsFired++;
            }
            return shotsFired >= 4 && GameObject.FindWithTag("Target") == null;
        });

        // Langkah 4: Reload
        HighlightElement(reloadButton.gameObject, "Ammo habis? Klik Reload!");
        yield return new WaitUntil(() => ammoManager.GetRemainingAmmo() == 4);

        // Langkah 5: Tembak Target Lagi
        target2.SetActive(true);
        HighlightElement(target2, "Tembak target ini!");
        yield return new WaitUntil(() => GameObject.FindWithTag("Target") == null);

        // Langkah 6: Selesai
        overlayPanel.SetActive(false);
        endText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        SceneManager.LoadScene("MainGame");
    }

    void HighlightElement(GameObject element, string message)
    {
        RectTransform elementRect = element.GetComponent<RectTransform>();
        Vector2 screenPos;

        if (elementRect != null) // UI Elements
        {
            screenPos = elementRect.anchoredPosition;
            highlightRect.sizeDelta = elementRect.sizeDelta + new Vector2(40f, 40f); // Padding
            handCursor.anchoredPosition = screenPos + new Vector2(elementRect.sizeDelta.x / 2 + 60f, -50); // Cursor di kanan
            Debug.Log($"UI Element: {element.name}, AnchoredPos: {screenPos}, Size: {elementRect.sizeDelta}");
        }
        else // World Space (target)
        {
            Vector3 worldPos = element.transform.position;
            Vector3 screenPos3D = Camera.main.WorldToScreenPoint(worldPos);
            RectTransform canvasRect = overlayPanel.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            Vector2 viewportPos = new Vector2(screenPos3D.x / Screen.width, screenPos3D.y / Screen.height);
            screenPos = new Vector2(viewportPos.x * canvasRect.sizeDelta.x - canvasRect.sizeDelta.x / 2,
                                    viewportPos.y * canvasRect.sizeDelta.y - canvasRect.sizeDelta.y / 2);
            highlightRect.sizeDelta = new Vector2(150f, 150f); // Ukuran target
            handCursor.anchoredPosition = screenPos + new Vector2(80f, 0); // Cursor di kanan
            Debug.Log($"World Element: {element.name}, WorldPos: {worldPos}, ScreenPos: {screenPos3D}, CanvasPos: {screenPos}");
        }

        highlightRect.anchoredPosition = screenPos;
        highlightBox.SetActive(true);
        handCursor.gameObject.SetActive(true);
    }
}