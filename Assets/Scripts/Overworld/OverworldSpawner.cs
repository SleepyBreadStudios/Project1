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

    [SerializeField]
    private GameObject structure = null;

    // Allows to set number of objects to spawn in world
    [SerializeField]
    private int numOfSpawns = 0;

    void Start() {
        
    }

    public void Spawn()
    {
        StartCoroutine(spawnObject(2.0f, structure));
    }

    private IEnumerator spawnObject(float interval, GameObject structure) {
        yield return new WaitForSeconds(interval);
        GameObject newObject = Instantiate(structure, new Vector3(Random.Range(-10f, 10), Random.Range(-10f, 10f), 0), Quaternion.identity);
        newObject.GetComponent<NetworkObject>().Spawn();
        StartCoroutine(spawnObject(2.0f, structure));
    }
}
