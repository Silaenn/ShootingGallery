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
    [SerializeField] TextMeshProUGUI instructionText;
    [SerializeField] GameObject target1;
    [SerializeField] GameObject target2;
    [SerializeField] GameObject crossHair;
    [SerializeField] TextMeshProUGUI endText;
    [SerializeField] GunShoot gunShoot;
    [SerializeField] GunMovement gunMovement;
    [SerializeField] AmmoManager ammoManager;

    [SerializeField] float stepDuration = 4f;
    RectTransform highlightRect;
    Transform originalTimerParent;
    Vector2 originalTimerPosition;
    int originalTimerSiblingIndex;
    Transform originalScoreParent;
    Vector2 originalScorePosition;
    int originalScoreSiblingIndex;
    Transform originalReloadParent;
    Vector2 originalReloadPosition;
    int originalReloadSiblingIndex;

    GameObject[] targets = new GameObject[4];

    void Start()
    {
        highlightRect = highlightBox.GetComponent<RectTransform>();

        handCursor.SetParent(highlightRect, false);
        overlayPanel.SetActive(true);
        highlightBox.SetActive(false);
        handCursor.gameObject.SetActive(false);
        instructionText.gameObject.SetActive(false);
        target1.SetActive(false);
        target2.SetActive(false);
        endText.gameObject.SetActive(false);
        crossHair.SetActive(false);

        if (overlayPanel != null)
        {
            overlayPanel.transform.SetAsLastSibling();
        }
        if (gunShoot != null) gunShoot.enabled = false;
        if (gunMovement != null) gunMovement.enabled = false;
        crossHair.GetComponent<CrosshairController>().enabled = false;

        if (timerText != null)
        {
            originalTimerParent = timerText.transform.parent;
            originalTimerPosition = timerText.GetComponent<RectTransform>().anchoredPosition;
            originalTimerSiblingIndex = timerText.transform.GetSiblingIndex();
        }

        if (scoreText != null)
        {
            originalScoreParent = scoreText.transform.parent;
            originalScorePosition = scoreText.GetComponent<RectTransform>().anchoredPosition;
            originalScoreSiblingIndex = scoreText.transform.GetSiblingIndex();
        }

        if (reloadButton != null)
        {
            originalReloadParent = reloadButton.transform.parent;
            originalReloadPosition = reloadButton.GetComponent<RectTransform>().anchoredPosition;
            originalReloadSiblingIndex = reloadButton.transform.GetSiblingIndex();
        }

        StartCoroutine(RunTutorial());
    }

    System.Collections.IEnumerator RunTutorial()
    {
        HighlightElement(timerText.gameObject, "Ini waktu permainan!", true);
        yield return new WaitForSecondsRealtime(stepDuration);
        ResetElement(timerText.gameObject);

        HighlightElement(scoreText.gameObject, "Ini skor kamu!", true);
        yield return new WaitForSecondsRealtime(stepDuration);
        ResetElement(scoreText.gameObject);

        overlayPanel.SetActive(false);
        if (gunShoot != null)
        {
            gunShoot.enabled = true;
            gunShoot.ResetShoot();
        }
        if (gunMovement != null) gunMovement.enabled = true;
        if (crossHair != null) crossHair.SetActive(true);
        crossHair.GetComponent<CrosshairController>().enabled = true;


        for (int i = 0; i < 4; i++)
        {
            if (target1 != null)
            {
                targets[i] = Instantiate(target1, new Vector3(i * 2f, 0f, 0f), Quaternion.identity);
                targets[i].SetActive(true);
            }
        }
        HighlightElement(targets[0], "Tembak target");
        yield return new WaitUntil(() =>
        {
            foreach (GameObject target in targets)
            {
                if (target != null && target.activeInHierarchy)
                {
                    return false;
                }
            }
            return true;
        });

        overlayPanel.SetActive(true);
        HighlightElement(reloadButton.gameObject, "Ammo habis? Klik Reload!", true);
        yield return new WaitUntil(() => ammoManager.GetRemainingAmmo() == 4);
        ResetElement(reloadButton.gameObject);

        overlayPanel.SetActive(false);
        target2.SetActive(true);
        HighlightElement(target2, "Tembak target ini!");
        yield return new WaitUntil(() => GameObject.FindWithTag("Target") == null);

        overlayPanel.SetActive(true);
        endText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        SceneManager.LoadScene("MainGame");
    }

    void HighlightElement(GameObject element, string message, bool makeChildOfOverlay = false)
    {
        RectTransform elementRect = element.GetComponent<RectTransform>();
        Vector2 screenPos;

        if (elementRect != null)
        {
            TextMeshProUGUI tmp = element.GetComponent<TextMeshProUGUI>();
            screenPos = elementRect.anchoredPosition;

            if (makeChildOfOverlay && overlayPanel != null)
            {
                element.transform.SetParent(overlayPanel.transform, false);
            }

            if (tmp != null)
            {
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.margin = Vector4.zero;

                float paddingX;
                float paddingY = 20f;
                paddingX = (element == reloadButton.gameObject) ? 50f : 100f;

                Vector2 textSize = new Vector2(tmp.preferredWidth + paddingX, tmp.preferredHeight + paddingY);
                highlightRect.sizeDelta = textSize;

                highlightRect.anchorMin = elementRect.anchorMin;
                highlightRect.anchorMax = elementRect.anchorMax;
                highlightRect.pivot = elementRect.pivot;

                highlightRect.anchoredPosition = screenPos;
                handCursor.anchoredPosition = screenPos + new Vector2(textSize.x / 2 + 60f, -textSize.y / 2);

                Debug.Log($"TMP Element: {element.name}, Preferred Size: ({tmp.preferredWidth}, {tmp.preferredHeight}), Highlight Size: {highlightRect.sizeDelta}, Pos: {highlightRect.anchoredPosition}");
            }
            else
            {
                highlightRect.sizeDelta = new Vector2(elementRect.sizeDelta.x - 20f, elementRect.sizeDelta.y);
                highlightRect.anchorMin = elementRect.anchorMin;
                highlightRect.anchorMax = elementRect.anchorMax;
                highlightRect.pivot = elementRect.pivot;
                highlightRect.anchoredPosition = screenPos;
                handCursor.anchoredPosition = screenPos + new Vector2(elementRect.sizeDelta.x / 2 + 30f, -elementRect.sizeDelta.y / 2);

                Debug.Log($"UI Element: {element.name}, Size: {elementRect.sizeDelta}, Highlight Size: {highlightRect.sizeDelta}, Pos: {highlightRect.anchoredPosition}");
            }
        }
        else
        {
            Vector3 worldPos = element.transform.position;
            Vector3 screenPos3D = Camera.main.WorldToScreenPoint(worldPos);
            RectTransform canvasRect = overlayPanel.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            Vector2 viewportPos = new Vector2(screenPos3D.x / Screen.width, screenPos3D.y / Screen.height);
            screenPos = new Vector2(viewportPos.x * canvasRect.sizeDelta.x - canvasRect.sizeDelta.x / 2,
                                    viewportPos.y * canvasRect.sizeDelta.y - canvasRect.sizeDelta.y / 2);

            highlightRect.sizeDelta = new Vector2(150f, 150f); // Ukuran tetap untuk target
            highlightRect.anchorMin = new Vector2(0.5f, 0.5f); // Anchor tengah
            highlightRect.anchorMax = new Vector2(0.5f, 0.5f);
            highlightRect.pivot = new Vector2(0.5f, 0.5f);    // Pivot tengah
            highlightRect.anchoredPosition = screenPos;
            handCursor.anchoredPosition = screenPos + new Vector2(80f, 0);

            Debug.Log($"World Element: {element.name}, WorldPos: {worldPos}, ScreenPos: {screenPos3D}, CanvasPos: {screenPos}");
        }

        highlightBox.SetActive(true);
        handCursor.gameObject.SetActive(true);
        instructionText.gameObject.SetActive(true);
        instructionText.text = message;
    }

    void ResetElement(GameObject element)
    {
        RectTransform elementRect = element.GetComponent<RectTransform>();
        if (elementRect == null) return;

        if (element == timerText.gameObject && originalTimerParent != null)
        {
            element.transform.SetParent(originalTimerParent, false);
            elementRect.anchoredPosition = originalTimerPosition;
            element.transform.SetSiblingIndex(originalTimerSiblingIndex);
        }
        else if (element == scoreText.gameObject && originalScoreParent != null)
        {
            element.transform.SetParent(originalScoreParent, false);
            elementRect.anchoredPosition = originalScorePosition;
            element.transform.SetSiblingIndex(originalScoreSiblingIndex);
        }
        else if (element == reloadButton.gameObject && originalReloadParent != null)
        {
            element.transform.SetParent(originalReloadParent, false);
            elementRect.anchoredPosition = originalReloadPosition;
            element.transform.SetSiblingIndex(originalReloadSiblingIndex);
        }

        highlightBox.SetActive(false);
        handCursor.gameObject.SetActive(false);
        instructionText.gameObject.SetActive(false);
    }
}