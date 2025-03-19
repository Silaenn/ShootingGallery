using System;
using System.Collections;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] bawahPrefabs;
    public GameObject[] tengahPrefabs;
    public float spawnInterval = 3f;
    public int initialBatchSize = 3;

    [Header("Difficulty Settings")]
    public int maxBatchSize = 5;
    public float minSpawnInterval = 1.5f;
    public float difficultyIncreaseRate = 0.1f;

    [Header("Ammo-Based Spawning")]
    public AmmoManager ammoManager;
    public bool useAmmoBasedSpawning = true;

    public Vector2 spawnRangeX = new Vector2(-6.84f, 6.84f);
    float[] spawnPositionsY = new float[] { -0.29f, -1.87f };

    private int currentBatchSize;
    private float currentSpawnInterval;
    private int waveCount = 0;
    private Coroutine spawnCoroutine;

    void Start()
    {

        currentBatchSize = initialBatchSize;
        currentSpawnInterval = spawnInterval;

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
        while (true)
        {
            int targetsToSpawn;

            if (useAmmoBasedSpawning && ammoManager != null)
            {
                int remainingAmmo = ammoManager.GetRemainingAmmo();

                if (remainingAmmo >= 3)
                {
                    targetsToSpawn = 3;
                }
                else if (remainingAmmo == 2)
                {
                    targetsToSpawn = 2;
                }
                else
                {
                    targetsToSpawn = 1;
                }
            }
            else
            {
                targetsToSpawn = currentBatchSize;
            }

            for (int i = 0; i < targetsToSpawn; i++)
            {
                SpawnTarget();
                yield return new WaitForSeconds(0.2f);
            }

            waveCount++;
            if (waveCount % 3 == 0)
            {
                IncreaseDifficulty();
            }

            yield return new WaitForSeconds(currentSpawnInterval);
        }
    }

    private void SpawnTarget()
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

        spawnCoroutine = StartCoroutine(SpawnTargets());
    }
}