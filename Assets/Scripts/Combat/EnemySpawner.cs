/******************************************************************************
 * Enemy spawner behavior file. Spawns in enemy objects
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;
    [SerializeField]
    private GameObject enemyPrefab2;
    [SerializeField]
    private GameObject enemyPrefab3;


    [SerializeField]
    private float enemyInterval = 4.0f;
    [SerializeField]
    private float enemyInterval2 = 6.0f;
    [SerializeField]
    private float enemyInterval3 = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Spawn()
    {
        StartCoroutine(spawnEnemy(enemyInterval, enemyPrefab));
        StartCoroutine(spawnEnemy(enemyInterval2, enemyPrefab2));
        StartCoroutine(spawnEnemy(enemyInterval3, enemyPrefab3));
    }

    private IEnumerator spawnEnemy(float interval, GameObject enemy) {
        yield return new WaitForSeconds(interval);
        GameObject newEnemy = Instantiate(enemy, new Vector3(Random.Range(-5f, 5), Random.Range(-6f, 6f), 0), Quaternion.identity);
        newEnemy.GetComponent<NetworkObject>().Spawn();
        StartCoroutine(spawnEnemy(interval, enemy));
    }
}
