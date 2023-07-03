using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BushBehavior : NetworkBehaviour
{
    [SerializeField]
    // type of drops the enemy drops when it dies(?)
    private GameObject item = null;

    private GameObject itemObj;

    private SpriteRenderer sprite;

    [SerializeField]
    private Sprite bushReady = null;

    [SerializeField]
    private Sprite bushHarvested = null;

    [SerializeField]
    private float spawnRadius = 2.0f;

    // how long it takes for bush to regenerate
    [SerializeField]
    private float RespawnTime;

    private bool currRegenerating = false;

    public GameObject GetItem()
    {
        return item;
    }

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void HarvestBerries()
	{
        if(currRegenerating)
		{
            return;
		}
        currRegenerating = true;
        ItemDrop();
        sprite.sprite = bushHarvested;
        StartCoroutine("Regenerate");
    }

    public void ItemDrop()
    {
        itemObj = Instantiate(item, new Vector3(Random.Range(transform.position.x - spawnRadius, transform.position.x + spawnRadius), 
            Random.Range(transform.position.y - spawnRadius, transform.position.y + spawnRadius), -1), Quaternion.identity) as GameObject;
        itemObj.GetComponent<NetworkObject>().Spawn(true);
    }

    // Regenerating object
    public IEnumerator Regenerate()
    {
        yield return new WaitForSeconds(2);
        currRegenerating = false;
        sprite.sprite = bushReady;
    }
}
