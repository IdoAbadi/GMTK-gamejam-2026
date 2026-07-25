using UnityEngine;
using System.Collections.Generic;

public class Can_spawner : MonoBehaviour
{
    [Header("Object Prefabs")]
    [Tooltip("Assign your 2 to 6 regular object prefabs here.")]
    public GameObject[] regularPrefabs;

    [Tooltip("Assign the single unique prize prefab here.")]
    public GameObject prizePrefab;

    [Header("Spawn Coordinates")]
    [Tooltip("The 3 specific coordinates where the prize is allowed to spawn.")]
    public List<Vector2> prizeSpawnLocations;

    [Tooltip("Coordinates for all the regular random objects.")]
    public List<Vector2> regularSpawnLocations;

    [Header("Settings")]
    [Tooltip("If true, the 2 prize locations that didn't get the prize will spawn regular objects instead.")]
    public bool fillEmptyPrizeLocations = false;

    void Start()
    {
        SpawnObjects();
    }

    private void SpawnObjects()
    {
        // 1. Handle the Prize Spawn
        if (prizePrefab != null && prizeSpawnLocations.Count > 0)
        {
            // Pick one random index from the 3 available prize locations
            int winningIndex = Random.Range(0, prizeSpawnLocations.Count);
            Vector2 prizePos = prizeSpawnLocations[winningIndex];

            // Instantiate the prize
            Instantiate(prizePrefab, prizePos, Quaternion.identity);

            // Optional: Fill the other 2 unused prize locations with regular objects
            if (fillEmptyPrizeLocations && regularPrefabs.Length > 0)
            {
                for (int i = 0; i < prizeSpawnLocations.Count; i++)
                {
                    if (i != winningIndex) // Skip the one where the prize just spawned
                    {
                        SpawnRandomRegularObject(prizeSpawnLocations[i]);
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("Prize Prefab or Prize Locations are missing!");
        }

        // 2. Handle the Regular Spawns
        if (regularPrefabs.Length > 0)
        {
            foreach (Vector2 pos in regularSpawnLocations)
            {
                SpawnRandomRegularObject(pos);
            }
        }
    }

    // Helper method to keep the code clean
    private void SpawnRandomRegularObject(Vector2 position)
    {
        int randomIndex = Random.Range(0, regularPrefabs.Length);
        GameObject selectedPrefab = regularPrefabs[randomIndex];

        Instantiate(selectedPrefab, position, Quaternion.identity);
    }
}