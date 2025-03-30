using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using Unity.VisualScripting;

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
    [SerializeField] Sprite highlightSprite;
    [SerializeField] Sprite handCursorSprite;
    [SerializeField] GameObject bacgroundTarget;
    [SerializeField] LayerMask targetLayerMask;


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
    Transform originalInstructionParent;
    Canvas mainCanvas;

    GameObject[] targets = new GameObject[4];
    GameObject[] worldHighlights;
    GameObject[] worldHandCursor;
    GameObject currentTrackedTarget;
    GameObject target2Instance;
    bool targetsActive = false;
    bool target2Active = false;
    int currentIndex = 0;


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

        originalInstructionParent = instructionText.transform.parent;
        mainCanvas = overlayPanel.GetComponentInParent<Canvas>();

        SpriteRenderer bgSr = bacgroundTarget.GetComponent<SpriteRenderer>();
        if (bgSr != null) bgSr.sortingOrder = 210;
        bacgroundTarget.SetActive(false);

        worldHighlights = new GameObject[4];
        worldHandCursor = new GameObject[4];

        if (overlayPanel != null)
        {
            overlayPanel.transform.SetAsLastSibling();
        }
        if (gunShoot != null) gunShoot.enabled = false;
        if (gunMovement != null) gunMovement.enabled = false;
        if (gunShoot != null)
        {
            gunShoot.targetLayerMask = targetLayerMask;
        }
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

    void Update()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] != null && worldHighlights[i] != null && worldHighlights[i].activeInHierarchy)
            {
                UpdateWorldHighlightPosition(targets[i], worldHighlights[i]);
                if (worldHandCursor[i] != null && worldHandCursor[i].activeInHierarchy)
                {
                    UpdateWorldHandCursorPosition(targets[i], worldHandCursor[i]);
                }
            }
        }

        if (currentTrackedTarget == target2Instance && target2Instance != null && worldHighlights[0] != null && worldHighlights[0].activeInHierarchy)
        {
            UpdateWorldHighlightPosition(target2Instance, worldHighlights[0]);
            if (worldHandCursor[0] != null && worldHandCursor[0].activeInHierarchy)
            {
                UpdateWorldHandCursorPosition(target2Instance, worldHandCursor[0]);
            }
        }

        if (gunShoot != null && gunShoot.HasShot)
        {
            Debug.Log("Memulai raycast...");
            try
            {
                Debug.Log($"Raycast selesai. Hit: {(gunShoot.hit.collider != null ? gunShoot.hit.collider.gameObject.name : "null")}");
                if (gunShoot.hit.collider != null && gunShoot.hit.collider.CompareTag("Target"))
                {
                    GameObject hitTarget = gunShoot.hit.collider.gameObject;

                    if (targetsActive)
                    {
                        Debug.Log("Targets aktif, memproses targets...");
                        for (int i = 0; i < targets.Length; i++)
                        {
                            if (targets[i] == hitTarget)
                            {
                                SpriteRenderer targetsSr = targets[i].GetComponent<SpriteRenderer>();
                                if (targetsSr != null)
                                {
                                    targetsSr.sortingOrder = 8;
                                }
                                if (worldHighlights[i] != null)
                                {
                                    Debug.Log($"Menonaktifkan WorldHighlight[{i}]");
                                    worldHighlights[i].SetActive(false);
                                }
                                if (worldHandCursor[i] != null)
                                {
                                    Debug.Log($"Menonaktifkan WorldHandCursor[{i}]");
                                    worldHandCursor[i].SetActive(false);
                                }
                                targets[i] = null;
                                break;
                            }
                        }
                    }

                    if (target2Active && hitTarget == target2Instance)
                    {
                        SpriteRenderer target2Sr = target2Instance.GetComponent<SpriteRenderer>();
                        if (target2Sr != null)
                        {
                            target2Sr.sortingOrder = 8;
                        }
                        if (worldHighlights[0] != null)
                        {
                            Debug.Log("Menonaktifkan WorldHighlight[0]");
                            worldHighlights[0].SetActive(false);
                        }
                        if (worldHandCursor[0] != null)
                        {
                            Debug.Log("Menonaktifkan WorldHandCursor[0]");
                            worldHandCursor[0].SetActive(false);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error di raycast: {e.Message}");
            }
            gunShoot.ResetShoot();
        }

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

        bacgroundTarget.SetActive(true);
        targetsActive = true;
        for (int i = 0; i < 4; i++)
        {
            if (target1 != null && targets[i] == null || !targets[i])
            {
                targets[i] = Instantiate(target1, new Vector3(i * 2f, 0f, 0f), Quaternion.identity);
                targets[i].SetActive(true);
                targets[i].layer = LayerMask.NameToLayer("Target");
                SpriteRenderer targetSr = targets[i].GetComponent<SpriteRenderer>();
                targetSr.sortingOrder = 212;
            }
        }
        Time.timeScale = 0.5f;
        HighlightElement(targets[0], "Tembak target");

        while (true)
        {
            yield return new WaitUntil(() =>
            {
                bool allTargetsDestroyed = true;
                foreach (GameObject target in targets)
                {
                    if (target != null && target.activeInHierarchy)
                    {
                        allTargetsDestroyed = false;
                        break;
                    }
                }

                if (allTargetsDestroyed) return true;

                if (ammoManager.GetRemainingAmmo() <= 0 && !allTargetsDestroyed)
                {
                    ammoManager.ReloadAmmo();

                    for (int i = 0; i < targets.Length; i++)
                    {
                        if (targets[i] == null)
                        {
                            targets[i] = Instantiate(target1, new Vector3(i * 2f, 0f, 0f), Quaternion.identity);
                            targets[i].SetActive(true);
                            targets[i].layer = LayerMask.NameToLayer("Target");
                            SpriteRenderer targetSr = targets[i].GetComponent<SpriteRenderer>();
                            if (targetSr != null)
                            {
                                targetSr.sortingOrder = 212;
                            }
                        }
                        else if (!targets[i].activeInHierarchy)
                        {
                            targets[i].SetActive(true);
                            targets[i].transform.position = new Vector3(i * 2f, 0f, 0f);
                            SpriteRenderer targetSr = targets[i].GetComponent<SpriteRenderer>();
                            if (targetSr != null)
                            {
                                targetSr.sortingOrder = 212;
                            }
                        }
                        else
                        {
                            targets[i].transform.position = new Vector3(i * 2f, 0f, 0f);
                        }
                    }

                    if (targets[0] != null)
                    {
                        HighlightElement(targets[0], "Tembak target");
                    }
                }
                return false;
            });

            if (AllTargetsDestroyed()) break;
        }

        targetsActive = false;
        Time.timeScale = 1f;
        instructionText.transform.SetParent(originalInstructionParent, false);

        overlayPanel.SetActive(true);
        bacgroundTarget.SetActive(false);
        HighlightElement(reloadButton.gameObject, "Ammo habis? Klik Reload!", true);
        yield return new WaitUntil(() => ammoManager.GetRemainingAmmo() == 4);
        ResetElement(reloadButton.gameObject);

        overlayPanel.SetActive(false);
        bacgroundTarget.SetActive(true);

        if (target2Instance == null)
        {
            target2Instance = Instantiate(target2, new Vector3(0f, 0f, 0f), Quaternion.identity);
            target2Instance.SetActive(true);
        }

        HighlightElement(target2Instance, "Tembak target ini!");
        SpriteRenderer target2SR = target2Instance.GetComponent<SpriteRenderer>();
        target2SR.sortingOrder = 212;
        Time.timeScale = 0.5f;
        target2Active = true;

        while (target2Instance != null && target2Instance.activeInHierarchy)
        {
            yield return new WaitUntil(() => ammoManager.GetRemainingAmmo() <= 0 || target2Instance == null);

            if (ammoManager.GetRemainingAmmo() <= 0 && target2Instance.activeInHierarchy)
            {
                ammoManager.ReloadAmmo();
                if (target2Instance == null || !target2Instance)
                {
                    target2Instance = Instantiate(target2, new Vector3(0f, 0f, 0f), Quaternion.identity);
                }
                target2Instance.SetActive(true);
                target2Instance.transform.position = new Vector3(0f, 0f, 0f);
                SpriteRenderer target2Sr = target2Instance.GetComponent<SpriteRenderer>();
                target2Sr.sortingOrder = 212;
                HighlightElement(target2Instance, "Tembak target ini!");
            }
        }

        target2Active = false;
        Time.timeScale = 1f;
        ResetElement(null);

        SurvivalTimer survivalTimer = FindObjectOfType<SurvivalTimer>();
        if (survivalTimer != null)
        {
            survivalTimer.StopTimer();
        }
        overlayPanel.SetActive(false);
        endText.gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(5f);
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

                handCursor.pivot = (element == timerText.gameObject) ? new Vector2(-1.03f, 1.51f) :
                   (element == scoreText.gameObject) ? new Vector2(-2.14f, 0.77f) :
                   (element == reloadButton.gameObject) ? new Vector2(-2.3599999f, 1.50999999f) : new Vector2(0.5f, 0.5f);

                handCursor.anchoredPosition = screenPos + new Vector2(textSize.x / 2 + 300f, 0);

                instructionText.rectTransform.pivot = (element == timerText.gameObject) ? new Vector2(0.5f, -0.15f) : (element == scoreText.gameObject) ? new Vector2(-0.430000007f, -1.23000002f) :
                 (element == reloadButton.gameObject) ? new Vector2(-3.1500001f, 7.11999989f) :
                 new Vector2(0.5f, 0.5f);
            }
            else
            {
                highlightRect.sizeDelta = new Vector2(elementRect.sizeDelta.x - 60f, elementRect.sizeDelta.y);
                highlightRect.anchorMin = elementRect.anchorMin;
                highlightRect.anchorMax = elementRect.anchorMax;
                highlightRect.pivot = elementRect.pivot;
                highlightRect.anchoredPosition = screenPos;
                handCursor.anchoredPosition = screenPos + new Vector2(elementRect.sizeDelta.x / 2 + 60f, -elementRect.sizeDelta.y / 2);

                instructionText.rectTransform.anchorMin = elementRect.anchorMin;
                instructionText.rectTransform.anchorMax = elementRect.anchorMax;
                instructionText.rectTransform.anchoredPosition = screenPos + new Vector2(0, elementRect.sizeDelta.y + 20f);
            }

            for (int i = 0; i < worldHighlights.Length; i++)
            {
                if (worldHighlights[i] != null) worldHighlights[i].SetActive(false);
                if (worldHandCursor[i] != null) worldHandCursor[i].SetActive(false);
            }

            highlightBox.SetActive(true);
            handCursor.gameObject.SetActive(true);
            instructionText.gameObject.SetActive(true);
        }
        else
        {
            currentTrackedTarget = element;
            instructionText.transform.SetParent(mainCanvas.transform, false);

            if (element == targets[0])
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    if (targets[i] != null && targets[i].activeInHierarchy)
                    {
                        if (worldHighlights[i] == null)
                        {
                            worldHighlights[i] = new GameObject("WorldHighlight_" + i);
                            SpriteRenderer sr = worldHighlights[i].AddComponent<SpriteRenderer>();
                            sr.sprite = highlightSprite;
                            sr.sortingLayerName = "Default";
                            sr.sortingOrder = 211;
                            worldHighlights[i].transform.localScale = new Vector3(1.5f, 1.5f, 1f);
                        }
                        worldHighlights[i].SetActive(true);
                        UpdateWorldHighlightPosition(targets[i], worldHighlights[i]);

                        if (worldHandCursor[i] == null)
                        {
                            worldHandCursor[i] = new GameObject("WorldHandCursor_" + i);
                            SpriteRenderer sr = worldHandCursor[i].AddComponent<SpriteRenderer>();
                            sr.sprite = handCursorSprite;
                            sr.sortingLayerName = "Default";
                            sr.sortingOrder = 213;
                            worldHandCursor[i].transform.localScale = new Vector3(0.219999999f, 0.219999999f, 0.219999999f);
                        }
                        worldHandCursor[i].SetActive(true);
                        UpdateWorldHandCursorPosition(targets[i], worldHandCursor[i]);
                    }
                    else
                    {
                        if (worldHighlights[i] != null) worldHighlights[i].SetActive(false);
                        if (worldHandCursor[i] != null) worldHandCursor[i].SetActive(false);
                    }
                }
            }

            else if (element == target2Instance)
            {
                if (target2Instance != null && target2Instance.activeInHierarchy)
                {
                    if (worldHighlights[0] == null)
                    {
                        worldHighlights[0] = new GameObject("WorldHighlight_0");
                        SpriteRenderer sr = worldHighlights[0].AddComponent<SpriteRenderer>();
                        sr.sprite = highlightSprite;
                        sr.sortingLayerName = "Default";
                        sr.sortingOrder = 211;
                        worldHighlights[0].transform.localScale = new Vector3(1.5f, 1.5f, 1f);
                    }
                    worldHighlights[0].SetActive(true);
                    UpdateWorldHighlightPosition(target2, worldHighlights[0]);

                    if (worldHandCursor[0] == null)
                    {
                        worldHandCursor[0] = new GameObject("WorldHandCursor_0");
                        SpriteRenderer sr = worldHandCursor[0].AddComponent<SpriteRenderer>();
                        sr.sprite = handCursorSprite;
                        sr.sortingLayerName = "Default";
                        sr.sortingOrder = 213;
                        worldHandCursor[0].transform.localScale = new Vector3(0.219999999f, 0.219999999f, 0.219999999f);
                    }
                    worldHandCursor[0].SetActive(true);
                    UpdateWorldHandCursorPosition(target2, worldHandCursor[0]);
                }
                else
                {
                    if (worldHighlights[0] != null) worldHighlights[0].SetActive(false);
                    if (worldHandCursor[0] != null) worldHandCursor[0].SetActive(false);
                }

                highlightBox.SetActive(false);
                handCursor.gameObject.SetActive(false);
            }

            highlightBox.gameObject.SetActive(true);
            handCursor.gameObject.SetActive(true);
            instructionText.gameObject.SetActive(true);

            instructionText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            instructionText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            instructionText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            instructionText.rectTransform.anchoredPosition = new Vector2(0, 300f);
            instructionText.alignment = TextAlignmentOptions.Center;
            instructionText.fontSize = 80;
        }
        instructionText.text = message;
    }

    void UpdateWorldHighlightPosition(GameObject target, GameObject worldHighlight)
    {
        Vector3 targetPos = target.transform.position;
        worldHighlight.transform.position = new Vector3(targetPos.x, targetPos.y, targetPos.z);
    }

    void UpdateWorldHandCursorPosition(GameObject target, GameObject worldHandCursor)
    {
        Vector3 targetPos = target.transform.position;
        worldHandCursor.transform.position = new Vector3(targetPos.x + 0.8f, targetPos.y + -0.5f, targetPos.z);
    }

    void ResetElement(GameObject element)
    {
        RectTransform elementRect = (element != null) ? element.GetComponent<RectTransform>() : null;
        if (elementRect == null)
        {
            currentTrackedTarget = null;
            for (int i = 0; i < worldHighlights.Length; i++)
            {
                if (worldHighlights[i] != null) worldHighlights[i].SetActive(false);
                if (worldHandCursor[i] != null) worldHandCursor[i].SetActive(false);
            }

            instructionText.transform.SetParent(originalInstructionParent, false);
            instructionText.gameObject.SetActive(false);
            handCursor.SetParent(overlayPanel.GetComponent<RectTransform>(), false);
        }
        else
        {
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
        }

        handCursor.gameObject.SetActive(false);
        instructionText.gameObject.SetActive(false);
        highlightBox.SetActive(false);
    }

    bool AllTargetsDestroyed()
    {
        foreach (GameObject target in targets)
        {
            if (target != null && target.activeInHierarchy)
            {
                return false;
            }
        }
        return true;
    }
}