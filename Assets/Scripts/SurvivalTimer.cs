using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SurvivalTimer : MonoBehaviour
{
    public float timeLeft = 60f;
    public int score = 0;
    public int scoreToBonus = 180;
    public float bonusTime = 10f;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public float timePerDifficultyIncrease = 15f;
    float difficultyTimer;
    int difficultyLevel = 1;
    [SerializeField] GameObject panelGameOver;

    public TargetSpawner targetSpawner;
    bool isGameOver = false;

    void Start()
    {
        difficultyTimer = timePerDifficultyIncrease;
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
        }


        if (timeLeft <= 0)
        {
            timeLeft = 0;
            GameOver();
        }

        if (score >= scoreToBonus)
        {
            AddBonusTime();
        }
    }

    void UpdateUI()
    {
        timerText.text = Mathf.Max(0, timeLeft).ToString("F0");
        scoreText.text = "Score: " + score;
        if (timeLeft < 5f) timerText.color = Color.red;
        else timerText.color = Color.white;
    }
    public void AddScore(int points)
    {
        score += points;
    }

    void AddBonusTime()
    {
        timeLeft += bonusTime;
        scoreToBonus += 180;
    }

    void IncreaseDifficulty()
    {
        difficultyLevel++;
        targetSpawner.IncreaseDifficulty();
    }

    void GameOver()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogError("AudioManager.Instance tidak ditemukan! Pastikan AudioManager ada di scene.");
            return;
        }

        isGameOver = true;
        AudioManager.Instance.StopBGM();
        panelGameOver.SetActive(true);
        Cursor.visible = true;

        var gunShoot = FindAnyObjectByType<GunShoot>();
        if (gunShoot != null) gunShoot.enabled = false;

        var gunMovement = FindAnyObjectByType<GunMovement>();
        if (gunMovement != null) gunMovement.enabled = false;

        var crosshairController = FindAnyObjectByType<CrosshairController>();
        if (crosshairController != null) crosshairController.enabled = false;

        var targetSpawner = FindAnyObjectByType<TargetSpawner>();
        if (targetSpawner != null) targetSpawner.enabled = false;

        Debug.Log("Game Over! Final Score: " + score);
    }
}
