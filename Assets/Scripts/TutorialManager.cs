using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] GameObject overlayPanel;
    [SerializeField] GameObject canvasNew;
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
    [SerializeField] UIGunMovement gunMovement;
    [SerializeField] AmmoManager ammoManager;
    [SerializeField] GameObject backgroundTarget;
    [SerializeField] LayerMask targetLayerMask;
    [SerializeField] SurvivalTimer survivalTimer;
    [SerializeField] float targetStartTime;

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
    bool tutorialStepCompleted = false;
    bool target2Shot = false;

    Color gordenOriginalColor;
    Color gordenRopeOriginalColor;
    int targetsShotCount = 0;

    // Object Pooling
    private List<GameObject> targetPool = new List<GameObject>();
    private const int POOL_SIZE = 5; // 4 targets + 1 target2

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

        SpriteRenderer bgSr = backgroundTarget.GetComponent<SpriteRenderer>();
        if (bgSr != null) bgSr.sortingOrder = 210;
        backgroundTarget.SetActive(false);

        worldHighlights = new GameObject[4];
        worldHandCursor = new GameObject[4];

        if (overlayPanel != null)
        {
            overlayPanel.transform.SetAsLastSibling();
        }
        if (gunShoot != null)
        {
            gunShoot.enabled = false;
            gunShoot.OnTargetDestroyed += OnTargetDestroyed;
            gunShoot.targetLayerMask = targetLayerMask;
        }
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

        if (survivalTimer == null)
        {
            survivalTimer = FindObjectOfType<SurvivalTimer>();
            if (survivalTimer == null) Debug.LogWarning("SurvivalTimer tidak ditemukan!");
        }

        InitializeTargetPool();
        StartCoroutine(RunTutorial());
    }

    // Inisialisasi object pool
    private void InitializeTargetPool()
    {
        for (int i = 0; i < POOL_SIZE; i++)
        {
            GameObject pooledTarget = Instantiate(target1, Vector3.zero, Quaternion.identity);
            pooledTarget.SetActive(false);
            targetPool.Add(pooledTarget);
        }
    }

    private GameObject GetPooledTarget()
    {
        foreach (var target in targetPool)
        {
            if (target != null && !target.activeInHierarchy) return target;
        }
        GameObject newTarget = Instantiate(target1, Vector3.zero, Quaternion.identity);
        newTarget.SetActive(false);
        targetPool.Add(newTarget);
        return newTarget;
    }

    void Update()
    {
        if (!targetsActive && !target2Active) return; // Optimasi: skip jika tidak aktif

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

        if (target2Active && target2Instance != null && target2Instance.activeInHierarchy)
        {
            if (Mathf.Abs(target2Instance.transform.position.x - -9.584623f) < 0.01f)
            {
                ResetTarget2();
            }
        }

        if (targetsActive)
        {
            bool shouldResetAll = false;

            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] != null && targets[i].activeInHierarchy && Mathf.Abs(targets[i].transform.position.x - -9.584623f) < 0.01f)
                {
                    shouldResetAll = true;
                    break;
                }
            }

            if (shouldResetAll)
            {
                ResetAllTargets();
            }
        }

        if (gunShoot != null && gunShoot.HasShot)
        {
            try
            {
                if (gunShoot.hit.collider != null && gunShoot.hit.collider.CompareTag("Target"))
                {
                    GameObject hitTarget = gunShoot.hit.collider.gameObject;

                    if (targetsActive)
                    {
                        for (int i = 0; i < targets.Length; i++)
                        {
                            if (targets[i] == hitTarget)
                            {
                                targetsShotCount++;
                                SpriteRenderer targetsSr = targets[i].GetComponent<SpriteRenderer>();
                                if (targetsSr != null)
                                {
                                    targetsSr.sortingOrder = 8;
                                }
                                if (worldHighlights[i] != null)
                                {
                                    worldHighlights[i].SetActive(false);
                                }
                                if (worldHandCursor[i] != null)
                                {
                                    worldHandCursor[i].SetActive(false);
                                }
                                break;
                            }
                        }
                    }

                    if (target2Active && hitTarget == target2Instance && !target2Shot)
                    {
                        target2Shot = true;
                        gunShoot.enabled = false;
                        gunMovement.enabled = false;
                        if (crossHair != null) crossHair.GetComponent<CrosshairController>().enabled = false;

                        SpriteRenderer target2Sr = target2Instance.GetComponent<SpriteRenderer>();
                        if (target2Sr != null)
                        {
                            target2Sr.sortingOrder = 8;
                        }
                        if (worldHighlights[0] != null)
                        {
                            worldHighlights[0].SetActive(false);
                        }
                        if (worldHandCursor[0] != null)
                        {
                            worldHandCursor[0].SetActive(false);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error di raycast: {e.Message}");
            }
        }
    }

    void ResetAllTargets()
    {
        targetsShotCount = 0;
        for (int i = 0; i < targets.Length; i++)
        {
            SpriteRenderer targetSr = targets[i]?.GetComponent<SpriteRenderer>();
            if (targetSr != null)
            {
                targetSr.sortingOrder = 225;
            }
            if (targets[i] != null)
            {
                targets[i].SetActive(false); // Nonaktifkan daripada Destroy
            }
            targets[i] = GetPooledTarget();
            targets[i].transform.position = new Vector3(i * 2f, 0f, 0f);
            targets[i].SetActive(true);
            targets[i].layer = LayerMask.NameToLayer("Target");
        }
        HighlightElement(targets[0], "Shoot Targets");
    }

    void ResetTarget2()
    {
        if (target2Instance != null)
        {
            target2Instance.SetActive(false); // Nonaktifkan daripada Destroy
        }
        target2Instance = GetPooledTarget();
        target2Instance.transform.position = new Vector3(0f, 0f, 0f);
        target2Instance.SetActive(true);
        SpriteRenderer target2Sr = target2Instance.GetComponent<SpriteRenderer>();
        if (target2Sr != null)
        {
            target2Sr.sortingOrder = 212;
        }
        target2Shot = false;
        if (gunShoot != null) gunShoot.enabled = true;
        HighlightElement(target2Instance, "Shoot this target!");
    }

    System.Collections.IEnumerator RunTutorial()
    {
        HighlightElement(timerText.gameObject, "It's game time", true);
        yield return new WaitForSecondsRealtime(stepDuration);
        ResetElement(timerText.gameObject);

        HighlightElement(scoreText.gameObject, "This is your score!", true);
        yield return new WaitForSecondsRealtime(stepDuration);
        ResetElement(scoreText.gameObject);

        overlayPanel.SetActive(false);
        if (gunShoot != null)
        {
            gunShoot.enabled = true;
        }
        if (gunMovement != null) gunMovement.enabled = true;
        if (crossHair != null) crossHair.SetActive(true);
        crossHair.GetComponent<CrosshairController>().enabled = true;

        backgroundTarget.SetActive(false);
        targetsActive = true;
        targetStartTime = survivalTimer.GetTime();
        for (int i = 0; i < 4; i++)
        {
            if (target1 != null && (targets[i] == null || !targets[i].activeInHierarchy))
            {
                targets[i] = GetPooledTarget();
                targets[i].transform.position = new Vector3(i * 2f, 0f, 0f);
                targets[i].SetActive(true);
                targets[i].layer = LayerMask.NameToLayer("Target");
                SpriteRenderer targetSr = targets[i].GetComponent<SpriteRenderer>();
                targetSr.sortingOrder = 212;
                targetSr.sortingLayerName = "Target";
            }
        }

        Time.timeScale = 0.5f;
        HighlightElement(targets[0], "Shoot Targets");

        while (!tutorialStepCompleted)
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

                if (allTargetsDestroyed)
                {
                    targetsActive = false;
                    tutorialStepCompleted = true;
                    for (int i = 0; i < worldHighlights.Length; i++)
                    {
                        if (worldHighlights[i] != null) worldHighlights[i].SetActive(false);
                        if (worldHandCursor[i] != null) worldHandCursor[i].SetActive(false);
                    }
                    return true;
                }

                if (ammoManager.GetRemainingAmmo() <= 0 && targetsShotCount < 4)
                {
                    ammoManager.ReloadAmmo();

                    bool targetsStillExist = false;
                    for (int i = 0; i < targets.Length; i++)
                    {
                        if (targets[i] != null && targets[i].activeInHierarchy)
                        {
                            targetsStillExist = true;
                        }

                        if (targets[i] == null)
                        {
                            targets[i] = GetPooledTarget();
                            targets[i].transform.position = new Vector3(i * 2f, 0f, 0f);
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

                    if (survivalTimer != null && targetsStillExist)
                    {
                        survivalTimer.SetTime(targetStartTime);
                    }

                    targetsShotCount = 0;
                    HighlightElement(targets[0], "Shoot Targets");
                }
                else if (ammoManager.GetRemainingAmmo() <= 0 && targetsShotCount >= 4)
                {
                    Debug.Log("Ammo habis, tapi semua target udah ditembak, nunggu hancur");
                }

                return false;
            });
        }

        targetsActive = false;
        Time.timeScale = 1f;
        instructionText.transform.SetParent(originalInstructionParent, false);

        overlayPanel.SetActive(true);
        backgroundTarget.SetActive(false);
        HighlightElement(reloadButton.gameObject, "Low ammo? Click Reload!", true);
        yield return new WaitUntil(() => ammoManager.GetRemainingAmmo() == 4);
        ResetElement(reloadButton.gameObject);

        overlayPanel.SetActive(false);
        // backgroundTarget.SetActive(true);

        if (target2Instance == null)
        {
            target2Instance = GetPooledTarget();
            target2Instance.transform.position = new Vector3(0f, 0f, 0f);
            target2Instance.layer = LayerMask.NameToLayer("Target");
            target2Instance.SetActive(true);
        }

        HighlightElement(target2Instance, "Shoot this target!");
        SpriteRenderer target2SR = target2Instance.GetComponent<SpriteRenderer>();
        target2SR.sortingOrder = 212;
        Time.timeScale = 0.5f;
        target2Active = true;

        while (target2Instance != null && target2Instance.activeInHierarchy)
        {
            yield return new WaitUntil(() => ammoManager.GetRemainingAmmo() <= 0 || target2Instance == null);

            if (ammoManager.GetRemainingAmmo() <= 0 && target2Instance != null && target2Instance.activeInHierarchy)
            {
                ammoManager.ReloadAmmo();
                if (target2Instance == null || !target2Instance.activeInHierarchy)
                {
                    target2Instance = GetPooledTarget();
                }
                target2Instance.transform.position = new Vector3(0f, 0f, 0f);
                target2Instance.SetActive(true);
                SpriteRenderer target2Sr = target2Instance.GetComponent<SpriteRenderer>();
                target2Sr.sortingOrder = 212;
                target2Shot = false;
                if (gunShoot != null) gunShoot.enabled = true;
                HighlightElement(target2Instance, "Shoot this target!");
            }

            Cursor.visible = false;
        }

        target2Active = false;
        Time.timeScale = 1f;
        ResetElement(null);

        if (survivalTimer != null)
        {
            survivalTimer.StopTimer();
        }

        overlayPanel.SetActive(false);
        if (canvasNew != null)
        {
            Canvas canvas = canvasNew.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.sortingOrder = 204;
            }
        }

        endText.gameObject.SetActive(true);
        SpriteRenderer backgroundTargetSr = backgroundTarget.GetComponent<SpriteRenderer>();
        if (backgroundTargetSr != null)
        {
            backgroundTargetSr.color = new Color(0f, 0f, 0f, 241f / 255f);
        }

        PlayerPrefs.SetInt("HasCompletedTutorial", 1);
        PlayerPrefs.Save();
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

            if (element == reloadButton.gameObject)
            {
                elementRect.pivot = new Vector2(0.50f, 0.5f);
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

                instructionText.rectTransform.pivot = (element == timerText.gameObject) ? new Vector2(0.5f, -0.15f) : (element == scoreText.gameObject) ? new Vector2(-0.05f, -1.63000002f) :
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

            Animator cursorAnimator = handCursor.GetComponent<Animator>();
            if (cursorAnimator != null)
            {
                cursorAnimator.Play("HandCursor", -1, 0f);
            }
            else
            {
                Debug.LogWarning("Animator tidak ditemukan di handCursor!");
            }
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
                        sr.sortingLayerName = "Default";
                        sr.sortingOrder = 211;
                        worldHighlights[0].transform.localScale = new Vector3(1.5f, 1.5f, 1f);
                    }
                    worldHighlights[0].SetActive(true);
                    UpdateWorldHighlightPosition(target2Instance, worldHighlights[0]);

                    if (worldHandCursor[0] == null)
                    {
                        worldHandCursor[0] = new GameObject("WorldHandCursor_0");
                        SpriteRenderer sr = worldHandCursor[0].AddComponent<SpriteRenderer>();
                        sr.sortingLayerName = "Default";
                        sr.sortingOrder = 213;
                        worldHandCursor[0].transform.localScale = new Vector3(0.219999999f, 0.219999999f, 0.219999999f);
                    }
                    worldHandCursor[0].SetActive(true);
                    UpdateWorldHandCursorPosition(target2Instance, worldHandCursor[0]);
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
            instructionText.fontSize = 73.89f;
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
                elementRect.pivot = new Vector2(0.5f, 0.5f);
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

    void OnTargetDestroyed(GameObject destroyedTarget)
    {
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] == destroyedTarget)
            {
                targets[i].SetActive(false);
                targets[i] = null;
                Debug.Log($"Target[{i}] dihancurkan oleh GunShoot");
            }
        }
        if (destroyedTarget == target2Instance)
        {
            target2Instance.SetActive(false);
            target2Instance = null;
            target2Shot = false;
            Debug.Log("Target2 dihancurkan oleh GunShoot");
        }
    }
}