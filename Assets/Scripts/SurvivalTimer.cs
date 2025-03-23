using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SurvivalTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] float timeLeft = 60f;
    [SerializeField] float bonusTime = 10f;
    [SerializeField] int scoreToBonus = 180;
    [SerializeField] float timePerDifficultyIncrease = 15f;

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
    int score = 0;
    float difficultyTimer;
    int difficultyLevel = 1;
    bool isGameOver = false;
    float lastTimeDIsplayed = -1f;
    const string HIGH_SCORE_KEY = "HighScore";

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
            timerText.color = timeLeft < 5f ? Color.red : Color.white;
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
            score += points;
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
    }

    void IncreaseDifficulty()
    {
        difficultyLevel++;
        if (targetSpawner != null)
        {
            targetSpawner.IncreaseDifficulty();
        }
    }

    void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;

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
}
