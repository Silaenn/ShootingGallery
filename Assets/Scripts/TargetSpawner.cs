using System;
using System.Collections;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] GameObject[] bawahPrefabs;
    [SerializeField] GameObject[] tengahPrefabs;
    [SerializeField] float spawnInterval = 3f;
    [SerializeField] int initialBatchSize = 3;
    [SerializeField] float initialBatchDelay = 0.2f;

    [Header("Difficulty Settings")]
    [SerializeField] int maxBatchSize = 5;
    [SerializeField] float minSpawnInterval = 1.5f;
    [SerializeField] float difficultyIncreaseRate = 0.1f;
    [SerializeField] float wavesPerDifficultyIncrease = 3f;

    [Header("Ammo-Based Spawning")]
    [SerializeField] AmmoManager ammoManager;
    [SerializeField] bool useAmmoBasedSpawning = true;

    [SerializeField] Vector2 spawnRangeX = new Vector2(-6.84f, 6.84f);
    [SerializeField] float[] spawnPositionsY = new float[] { -0.29f, -1.87f };

    int currentBatchSize;
    float currentSpawnInterval;
    int waveCount = 0;
    Coroutine spawnCoroutine;
    bool isSpawning = true;

    void Start()
    {

        currentBatchSize = initialBatchSize;
        currentSpawnInterval = spawnInterval;

        if (bawahPrefabs == null || bawahPrefabs.Length == 0 || tengahPrefabs == null || tengahPrefabs.Length == 0)
        {
            Debug.LogError("BawahPrefabs atau TengahPrefabs tidak diatur atau kosong! Spawner tidak akan bekerja.");
            enabled = false;
            return;
        }

        if (ammoManager == null)
        {
            ammoManager = FindObjectOfType<AmmoManager>();
            if (ammoManager == null)
            {
                Debug.LogError("AmmoManager tidak ditemukan! Nonaktifkan useAmmoBasedSpawning atau tambahkan AmmoManager.");
                useAmmoBasedSpawning = false;
            }
        }

        spawnCoroutine = StartCoroutine(SpawnTargets());
    }

    IEnumerator SpawnTargets()
    {
        while (isSpawning)
        {
            int targetsToSpawn = CalculateTargetsToSpawn();

            for (int i = 0; i < targetsToSpawn; i++)
            {
                SpawnTarget();
                yield return new WaitForSeconds(initialBatchDelay);
            }

            waveCount++;
            if (waveCount % wavesPerDifficultyIncrease == 0)
            {
                IncreaseDifficulty();
            }

            yield return new WaitForSeconds(currentSpawnInterval);
        }
    }

    int CalculateTargetsToSpawn()
    {
        if (useAmmoBasedSpawning && ammoManager != null)
        {
            int remainingAmmo = ammoManager.GetRemainingAmmo();

            if (remainingAmmo >= 3) return 3;
            if (remainingAmmo == 2) return 2;
            return 1;
        }

        return currentBatchSize;
    }

    void SpawnTarget()
    {
        float selectedY = spawnPositionsY[UnityEngine.Random.Range(0, spawnPositionsY.Length)];
        GameObject[] selectedPrefabs = (selectedY == -0.29f) ? tengahPrefabs : bawahPrefabs;

        if (selectedPrefabs.Length > 0)
        {
            GameObject randomPrefab = selectedPrefabs[UnityEngine.Random.Range(0, selectedPrefabs.Length)];
            Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(spawnRangeX.x, spawnRangeX.y), selectedY, 0);
            GameObject newTarget = Instantiate(randomPrefab, spawnPosition, Quaternion.identity);

            MovingTarget movingTarget = newTarget.GetComponent<MovingTarget>();
            if (movingTarget != null)
            {
                float speedMultiplier = UnityEngine.Random.Range(0.8f, 1.2f);
                movingTarget.moveSpeed *= speedMultiplier;
            }
        }
    }

    public void IncreaseDifficulty()
    {
        if (currentBatchSize < maxBatchSize)
        {
            currentBatchSize++;
        }

        currentSpawnInterval = Mathf.Max(minSpawnInterval, currentSpawnInterval - difficultyIncreaseRate);

        Debug.Log($"Kesulitan ditingkatkan: Batch={currentBatchSize}, Interval={currentSpawnInterval}");
    }

    public void ResetSpawner()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }

        currentBatchSize = initialBatchSize;
        currentSpawnInterval = spawnInterval;
        waveCount = 0;
        isSpawning = true;

        spawnCoroutine = StartCoroutine(SpawnTargets());
    }

    public void StopSpawning()
    {
        isSpawning = false;
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }
}