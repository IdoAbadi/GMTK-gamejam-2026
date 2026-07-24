using System;
using System.Collections;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] private GameObject foodPrefab;

    [SerializeField] private float leftSpawnLimit = -8f;
    [SerializeField] private float rightSpawnLimit = 8f;

    [SerializeField] private float minimumSpawnDelay = 0.6f;
    [SerializeField] private float maximumSpawnDelay = 1f;

    private void Start()
    {
        StartCoroutine(SpawnFoodRepeatedly());
    }

    private IEnumerator SpawnFoodRepeatedly()
    {
        while (true)
        {
            SpawnFood();

            float delay = UnityEngine.Random.Range(
                minimumSpawnDelay,
                maximumSpawnDelay
            );

            yield return new WaitForSeconds(delay);
        }
    }

    private void SpawnFood()
    {
        float randomX = UnityEngine.Random.Range(
            leftSpawnLimit,
            rightSpawnLimit
        );

        Vector3 spawnPosition = new Vector3(
            randomX,
            transform.position.y,
            0f
        );

        Instantiate(
            foodPrefab,
            spawnPosition,
            Quaternion.identity
        );
    }

    public void StopSpawning()
    {
        StopAllCoroutines();
    }
}