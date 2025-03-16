using System;
using System.Collections;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] bawahPrefabs;
    public GameObject[] tengahPrefabs;
    public float spawnInterval = 3f;
    public int batchSIze = 3;

    public Vector2 spawnRangeX = new Vector2(-6.84f, 6.84f);
    float[] spawnPositionsY = new float[] { -0.29f, -1.87f };


    void Start()
    {
        StartCoroutine(SpawnTargets());
    }

    IEnumerator SpawnTargets()
    {
        while (true)
        {
            for (int i = 0; i < batchSIze; i++)
            {
                SpawnTarget();
            }


            yield return new WaitForSeconds(spawnInterval);
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
            Instantiate(randomPrefab, spawnPosition, Quaternion.identity);
        }
    }
}