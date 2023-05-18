/******************************************************************************
* Shared template for items. Handles it's own deletion.
* 
* Authors: Alicia T, Jason N, Jino C
*****************************************************************************/
//#define Debug
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


// consider making abstract
public class ItemBehaviorSync : NetworkBehaviour
{
    public ItemData itemType;
    private Vector2 randLoc;
    private float immunity = 0.0f;
    //private readonly string[] itemPrefabs = { "capsule", "checkmark", "circle", "diamond", "flower", "hexagonpoint", "roundedsquare", "star", "triangle" };
    private SpawnerManager Spawner = null;

    // individual items can have their own count that alters based on players
    private int itemCount = 0;

    public ItemData GetItemType()
    {
        return itemType;
    }

    public int GetCount()
    {
        return itemCount;
    }

    public void SetCount(int newCount)
    {
        itemCount = newCount;
    }

    // on pick up delete, determined by player based on whether item was able to be picked up or not
    public void Delete()
    {
        Destroy(this.gameObject);
    }

    public void move()
    {
        float randy = Random.Range
                (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y);
        float randx = Random.Range
            (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x);

        randLoc = new Vector2(randx, randy);
    }
    void Awake()
    {
        Spawner = GameObject.Find("SpawnerManager").GetComponent<SpawnerManager>();
    }

    void Start()
    {
        itemCount = itemType.GetCount();
        move();
    }

    void Update()
    {
        // immunity timer up to determine whether object can get collision deleted
        immunity += Time.deltaTime;

        // Random movement of objects to test performance of code
        if (transform.position.x == randLoc.x && transform.position.y == randLoc.y)
        {
            move();
        }

        transform.position = Vector3.MoveTowards(transform.position, randLoc, Time.deltaTime * (float)0.3);

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Item") && immunity > 10.0f)
        {
            //string toLoad = "Items/" + itemPrefabs[Random.Range(0, 10)];
            //GameObject newItem = Instantiate(Resources.Load(toLoad, typeof(GameObject)), transform.position, Quaternion.identity) as GameObject;

            bool spawnedObject = Spawner.SpawnObjects(transform.position, this.GetComponent<NetworkObject>().NetworkObjectId, this.gameObject);
            //if (spawnedObject)
            //this.Delete();
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Spawner.AddObject(this.gameObject);
    }

    public override void OnNetworkDespawn()
    {
        Spawner.RemoveObject(this.gameObject);
        base.OnNetworkDespawn();
    }
}