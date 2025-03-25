using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SurvivalTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] float timeLeft = 60f;
    [SerializeField] float bonusTime = 10f;
    [SerializeField] int scoreToBonus = 50;
    [SerializeField] float timePerDifficultyIncrease = 20f;

    [Header("UI Elements")]
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] GameObject panelGameOver;
    [SerializeField] TextMeshProUGUI gameOverScoreText;
    [SerializeField] TextMeshProUGUI gameOverHighScoreText;

    [Header("References")]

    [SerializeField] TargetSpawner targetSpawner;
    [SerializeField] GunShoot gunShoot;
    [SerializeField] GunMovement gunMovement;
    [SerializeField] CrosshairController crosshairController;
    [SerializeField] AudioClip soundTimeOut;
    [SerializeField] AudioClip soundGameOver;

    [Header("Score Settings")]
    [SerializeField] float scoreMultiplier = 1f;
    [SerializeField] float multiplierIncreaseRate = 0.1f;

    [Header("Bonus Time Text Settings")]
    [SerializeField] GameObject bonusTimeTextPrefab;
    [SerializeField] float textOffsetX = 0f;
    [SerializeField] float textOffsetY = 2f;
    [SerializeField] float textDuration = 1f;

    int score = 0;
    float difficultyTimer;
    int difficultyLevel = 1;
    bool isGameOver = false;
    float lastTimeDIsplayed = -1f;
    const string HIGH_SCORE_KEY = "HighScore";
    AudioSource audioSource;
    bool hasPlayedTimeOutSound = false;

    void Start()
    {
        difficultyTimer = timePerDifficultyIncrease;

        if (timerText == null || scoreText == null || panelGameOver == null)
        {
            Debug.LogError("TimerText, ScoreText, atau PanelGameOver tidak diatur di Inspector!");
        }
        if (targetSpawner == null)
        {
            targetSpawner = FindObjectOfType<TargetSpawner>();
            if (targetSpawner == null) Debug.LogWarning("TargetSpawner tidak ditemukan!");
        }
        if (gunShoot == null) gunShoot = FindObjectOfType<GunShoot>();
        if (gunMovement == null) gunMovement = FindObjectOfType<GunMovement>();
        if (crosshairController == null) crosshairController = FindObjectOfType<CrosshairController>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        UpdateUI();
        panelGameOver.SetActive(false);
    }

    private void Update()
    {
        if (!isGameOver)
        {
            timeLeft -= Time.deltaTime;
            difficultyTimer -= Time.deltaTime;
            UpdateUI();

            if (difficultyTimer <= 0)
            {
                IncreaseDifficulty();
                difficultyTimer = timePerDifficultyIncrease;
            }

            if (timeLeft <= 0)
            {
                timeLeft = 0;
                GameOver();
            }
        }
    }

    void UpdateUI()
    {
        if (timerText != null && Mathf.Abs(timeLeft - lastTimeDIsplayed) > 0.1f)
        {
            timerText.text = Mathf.Max(0, timeLeft).ToString("F0");
            if (timeLeft < 5f)
            {
                timerText.color = Color.red;

                if (!hasPlayedTimeOutSound && audioSource != null && audioSource != null)
                {
                    audioSource.PlayOneShot(soundTimeOut);
                    hasPlayedTimeOutSound = true;
                }
            }
            else
            {
                timerText.color = Color.white;
                hasPlayedTimeOutSound = false;
            }
            lastTimeDIsplayed = timeLeft;
        }
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }
    public void AddScore(int points)
    {
        if (!isGameOver)
        {
            float adjustedPoints = points * scoreMultiplier;
            score += Mathf.RoundToInt(adjustedPoints);
            if (score >= scoreToBonus)
            {
                AddBonusTime();
            }
            UpdateUI();
        }
    }

    void AddBonusTime()
    {
        timeLeft += bonusTime;
        scoreToBonus += 180;
        SpawnBonusTimeText();
    }

    void SpawnBonusTimeText()
    {
        if (bonusTimeTextPrefab != null && timerText != null)
        {
            GameObject bonusTextObj = Instantiate(bonusTimeTextPrefab, timerText.transform.parent);

            RectTransform bonusRect = bonusTextObj.GetComponent<RectTransform>();
            RectTransform timerRect = timerText.GetComponent<RectTransform>();
            if (bonusRect != null && timerRect != null)
            {
                bonusRect.anchoredPosition = timerRect.anchoredPosition + new Vector2(textOffsetX, textOffsetY);
                bonusRect.anchorMin = timerRect.anchorMin;
                bonusRect.anchorMax = timerRect.anchorMax;
                bonusRect.pivot = new Vector2(0.5f, 0.5f);
            }

            TextMeshProUGUI bonusText = bonusTextObj.GetComponent<TextMeshProUGUI>();
            if (bonusText != null)
            {
                bonusText.text = "+" + bonusTime.ToString("F0");
                Debug.Log($"Bonus time text spawned at {bonusRect.anchoredPosition} with value: +{bonusTime}");
            }
            else
            {
                Debug.LogError("TextMeshPro component tidak ditemukan di bonusTimeTextPrefab!");
            }

            StartCoroutine(AnimateBonusTimeText(bonusTextObj));
        }
        else
        {
            Debug.LogWarning("bonusTimeText Prefab atau timerText tidak diatur di Inspector");
        }
    }

    IEnumerator AnimateBonusTimeText(GameObject textObj)
    {
        float elapsed = 0f;
        RectTransform rectTransform = textObj.GetComponent<RectTransform>();
        Vector2 startPos = rectTransform.anchoredPosition;
        TextMeshProUGUI bonusText = textObj.GetComponent<TextMeshProUGUI>();

        while (elapsed < textDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / textDuration;
            rectTransform.anchoredPosition = startPos + new Vector2(0, t * 50f);
            if (bonusText != null) bonusText.alpha = 1 - t;
            yield return null;
        }

        Destroy(textObj);
    }
    void IncreaseDifficulty()
    {
        difficultyLevel++;
        scoreMultiplier += multiplierIncreaseRate;
        if (targetSpawner != null)
        {
            targetSpawner.IncreaseDifficulty();
        }
    }

    void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;

        if (audioSource != null && soundGameOver != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(soundGameOver);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopBGM();
        }
        else
        {
            Debug.LogError("AudioManager.Instance tidak ditemukan! Pastikan AudioManager ada di scene.");
        }

        if (panelGameOver != null) panelGameOver.SetActive(true);
        Cursor.visible = true;

        if (gunShoot != null) gunShoot.enabled = false;

        if (gunMovement != null) gunMovement.enabled = false;

        if (crosshairController != null) crosshairController.enabled = false;

        if (targetSpawner != null)
        {
            targetSpawner.enabled = false;
            targetSpawner.StopSpawning();
        }

        UpdateGameOverUI();

        Debug.Log("Game Over! Final Score: " + score);
    }

    void UpdateGameOverUI()
    {
        int highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, highScore);
            PlayerPrefs.Save();
        }

        if (gameOverScoreText != null)
        {
            gameOverScoreText.text = "Score : " + score;
        }

        if (gameOverHighScoreText != null)
        {
            gameOverHighScoreText.text = "HighScore : " + highScore;
        }

        if (score == highScore)
        {
            gameOverHighScoreText.color = Color.yellow;
        }
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    public int GetDifficultyLevel()
    {
        return difficultyLevel;
    }
}
