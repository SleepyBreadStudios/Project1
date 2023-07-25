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

    public GameObject bounds;

    // Allows to set number of objects to spawn in world
    [SerializeField]
    private int numOfSpawns = 0;

    void Start() {
        spawnObject();
    }

    // public void Spawn()
    // {
    //     StartCoroutine(spawnObject(4.0f, structure));
    //     StartCoroutine(spawnObject(4.0f, structure2));
    // }

    // private IEnumerator spawnObject(float interval, GameObject s) {
    //     yield return new WaitForSeconds(interval);
    //     GameObject newObject = Instantiate(s, new Vector3(Random.Range(-10f, 10), Random.Range(-10f, 10f), 0), Quaternion.identity);
    //     newObject.GetComponent<NetworkObject>().Spawn();
    //     StartCoroutine(spawnObject(4.0f, s));
    // }

    public void spawnObject()
    {
        MeshCollider c = bounds.GetComponent<MeshCollider>();
        float screenX, screenY;
        Vector2 pos;
        for (int i = 0; i < numOfSpawns; i++) {
            screenX = Random.Range(c.bounds.min.x, c.bounds.max.x);
            screenY = Random.Range(c.bounds.min.y, c.bounds.max.y);
            pos = new Vector2(screenX, screenY);

            Instantiate(structure, pos, structure.transform.rotation);

        }
    }


    private void DestroyObjects()
    {
        foreach (GameObject o in GameObject.FindGameObjectsWithTag("Overworld"))
        {
            Destroy(o);
        }
    }
}
