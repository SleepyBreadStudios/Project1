/******************************************************************************
 * Overworld script that spawns in random objects for the world generation
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class OverworldSpawner : NetworkBehaviour
{
    public List<GameObject> structure;
    public GameObject spawnPoint;

    // Allows to set the number of objects to spawn in the world
    [SerializeField]
    private int numOfSpawns = 0;

    void Start()
    {
        SpawnObjects();
    }

    public void SpawnObjects()
    {
        // Use UnityEngine.Bounds instead of custom Bounds
        UnityEngine.Bounds cBounds = spawnPoint.GetComponent<MeshCollider>().bounds;
        GameObject spawn;

        // Create a list to store the positions of already spawned objects
        List<Vector2> spawnedPositions = new List<Vector2>();

        for (int i = 0; i < numOfSpawns; i++)
        {
            int rand = Random.Range(0, structure.Count);
            spawn = structure[rand];

            // Attempt to find a non-overlapping position
            Vector2 randomPosition = FindNonOverlappingPosition(cBounds, spawn.transform.localScale, spawnedPositions);

            // Instantiate the object if a suitable position is found
            if (randomPosition != Vector2.zero)
            {
                Instantiate(spawn, randomPosition, spawn.transform.rotation);
                spawnedPositions.Add(randomPosition); // Add the position to the list of spawned positions
            }
        }
    }

    private Vector2 FindNonOverlappingPosition(UnityEngine.Bounds bounds, Vector2 objectSize, List<Vector2> spawnedPositions)
    {
        // Define the number of attempts to find a non-overlapping position
        int maxAttempts = 100;
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            // Generate a random position within the bounds
            float screenX = Random.Range(bounds.min.x, bounds.max.x);
            float screenY = Random.Range(bounds.min.y, bounds.max.y);
            Vector2 randomPosition = new Vector2(screenX, screenY);

            // Check if the object overlaps with any other existing objects
            if (!IsOverlappingOtherObjects(randomPosition, objectSize, spawnedPositions))
            {
                return randomPosition; // Found a non-overlapping position
            }

            attempts++;
        }

        // Return Vector2.zero if no non-overlapping position is found after maxAttempts
        return Vector2.zero;
    }

    private bool IsOverlappingOtherObjects(Vector2 position, Vector2 objectSize, List<Vector2> spawnedPositions)
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(position, objectSize, 0f);

        // Check if any colliders were found
        if (colliders.Length > 0)
        {
            // Check if any of the colliders belong to the spawned objects
            foreach (var collider in colliders)
            {
                if (spawnedPositions.Contains(collider.transform.position))
                {
                    return true; // Overlapping with a spawned object
                }
            }
        }

        return false; // No overlapping with spawned objects
    }

    private void DestroyObjects()
    {
        foreach (GameObject o in GameObject.FindGameObjectsWithTag("Overworld"))
        {
            Destroy(o);
        }
    }
}

