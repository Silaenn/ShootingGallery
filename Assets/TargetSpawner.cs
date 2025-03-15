using System.Collections;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] targetPrefabs;
    public float spawnInterval = 3f;
    public int batchSIze = 3;

    public Vector2 spawnRangeX = new Vector2(-6.84f, 6.84f);
    public float[] spawnPositionsY = new float[] { -1.362507f, -0.0f };


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
                int randomIndex = Random.Range(0, targetPrefabs.Length);
                GameObject randomPrefab = targetPrefabs[randomIndex];

                if (randomPrefab != null)
                {
                    Instantiate(randomPrefab, GetRandomPosition(), Quaternion.identity);
                }
            }


            yield return new WaitForSeconds(spawnInterval);
        }
    }

    Vector3 GetRandomPosition()
    {
        float randomX = UnityEngine.Random.Range(spawnRangeX.x, spawnRangeX.y);

        float randomY = spawnPositionsY[UnityEngine.Random.Range(0, spawnPositionsY.Length)];
        return new Vector3(randomX, randomY, 0);
    }
}