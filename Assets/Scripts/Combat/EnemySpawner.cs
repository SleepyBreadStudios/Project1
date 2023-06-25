/******************************************************************************
 * Enemy spawner behavior file. Spawns in enemy objects
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using static UnityEditor.PlayerSettings;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;

    // spawn radius for spawning enemies
    [SerializeField]
    private float spawnRadius = 5.0f;

    // polling radius for counting enemies
    [SerializeField]
    private float pollRadius = 15.0f;

    // rate at which spawner polls surrounding area and determines spawn
    [SerializeField]
    private float pollRate = 10.0f;

    // limit for number of enemies in polling radius
    [SerializeField]
    private int limit = 5;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(spawnEnemy(pollRate, enemyPrefab));
    }

    private IEnumerator spawnEnemy(float interval, GameObject enemy) {
        yield return new WaitForSeconds(interval);

        float x = transform.position.x;
        float y = transform.position.y;

        // counter to see how many enemies are within radius, in the future possibly restrict by enemy type
        int count = 0;
        foreach (var e in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (e != null)
            {
                Vector2 loc = e.transform.position;
                if (loc.x < x + pollRadius && loc.x > x - pollRadius &&
                    loc.y < y + pollRadius && loc.y > y - pollRadius)
                {
                    count++;
                }
            }
        }

        Debug.Log("Spawner counting: " + count);

        // if count is less than limit, spawn an enemy inside spawning radius
        if (count < limit)
        {
            GameObject newEnemy = Instantiate(enemy, new Vector2(Random.Range(x - spawnRadius, x + spawnRadius), 
                Random.Range(y - spawnRadius, y + spawnRadius)), Quaternion.identity);
            newEnemy.GetComponent<NetworkObject>().Spawn();
        }

        StartCoroutine(spawnEnemy(interval, enemy));
    }
}
