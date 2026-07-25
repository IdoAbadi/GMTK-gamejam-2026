using System;
using UnityEngine;

public class CatToyRandomizer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject[] toys;
    [SerializeField] private BoxCollider2D spawnArea;

    [Header("Placement")]
    [SerializeField] private float extraSpacing = 0.2f;
    [SerializeField] private int maximumAttempts = 200;

    private void Start()
    {
        RandomizeToyPositions();
    }

    private void RandomizeToyPositions()
    {
        if (spawnArea == null || toys == null || toys.Length == 0)
        {
            Debug.LogError("Spawn Area or Toys are not assigned.");
            return;
        }

        Collider2D[] toyColliders = new Collider2D[toys.Length];
        float[] toyRadii = new float[toys.Length];

        for (int i = 0; i < toys.Length; i++)
        {
            if (toys[i] == null)
                continue;

            toyColliders[i] = toys[i].GetComponent<Collider2D>();

            if (toyColliders[i] == null)
            {
                Debug.LogError(toys[i].name + " has no Collider 2D.");
                continue;
            }

            toyRadii[i] = Mathf.Max(
                toyColliders[i].bounds.extents.x,
                toyColliders[i].bounds.extents.y
            );

            toyColliders[i].enabled = false;

            Rigidbody2D rb = toys[i].GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }

        for (int i = 0; i < toys.Length; i++)
        {
            if (toys[i] == null || toyColliders[i] == null)
                continue;

            bool positionFound = false;
            float checkRadius = toyRadii[i] + extraSpacing;

            for (int attempt = 0; attempt < maximumAttempts; attempt++)
            {
                Vector2 candidate = GetRandomPosition(toyRadii[i]);

                if (!CanPlaceToy(candidate, checkRadius))
                    continue;

                toys[i].transform.position = new Vector3(
                    candidate.x,
                    candidate.y,
                    toys[i].transform.position.z
                );

                toyColliders[i].enabled = true;
                positionFound = true;
                break;
            }

            if (!positionFound)
            {
                toyColliders[i].enabled = true;

                Debug.LogWarning(
                    "Could not find a free position for " + toys[i].name
                );
            }
        }
    }

    private Vector2 GetRandomPosition(float toyRadius)
    {
        Bounds bounds = spawnArea.bounds;

        float x = UnityEngine.Random.Range(
            bounds.min.x + toyRadius,
            bounds.max.x - toyRadius
        );

        float y = UnityEngine.Random.Range(
            bounds.min.y + toyRadius,
            bounds.max.y - toyRadius
        );

        return new Vector2(x, y);
    }

    private bool CanPlaceToy(Vector2 position, float radius)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, radius);

        foreach (Collider2D hit in hits)
        {
            if (hit == spawnArea)
                continue;

            return false;
        }

        return true;
    }
}