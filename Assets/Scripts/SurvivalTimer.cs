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
    private float difficultyTimer;
    private int difficultyLevel = 1;

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
        isGameOver = true;
        Time.timeScale = 0;
        Debug.Log("Game Over! Final Score: " + score);
    }
}
