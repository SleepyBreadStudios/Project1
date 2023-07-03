using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PondBehavior : NetworkBehaviour
{
    [SerializeField]
    // type of drops the enemy drops when it dies(?)
    private GameObject item = null;

    private GameObject itemObj;

    private SpriteRenderer sprite;

    [SerializeField]
    private Sprite pond = null;

	[SerializeField]
	private Sprite pondFish = null;

    [SerializeField]
    private Sprite pondCurrFish = null;

    [SerializeField]
    private float spawnRadius = 2.0f;

    // how long it takes for bush to regenerate
    [SerializeField]
    private float countdown = 1.0f;

    private bool currFishing = false;

    private bool withinFishTime = false;

    public GameObject GetItem()
    {
        return item;
    }

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!currFishing)
        {
            StopCoroutine("StartFishTimer");
            StopCoroutine("StartFishCountdown");
            sprite.sprite = pond;
        }

    }

    public void Fish()
    {
        if (withinFishTime)
        {
            ItemDrop();
            currFishing = false;
            sprite.sprite = pond;
            withinFishTime = false;
        }
        if (!currFishing)
        {
            StartCoroutine("StartFishTimer");
            sprite.sprite = pondCurrFish;
            currFishing = true;
        }
    }

    public void ItemDrop()
    {
        itemObj = Instantiate(item, new Vector3(Random.Range(transform.position.x - spawnRadius, transform.position.x + spawnRadius),
            Random.Range(transform.position.y - spawnRadius, transform.position.y + spawnRadius), -1), Quaternion.identity) as GameObject;
        itemObj.GetComponent<NetworkObject>().Spawn(true);
    }

    // Regenerating object
    public IEnumerator StartFishTimer()
    {
        yield return new WaitForSeconds(Random.Range(0.5f, 3f));
        withinFishTime = true;
        Debug.Log("Counting for fish");
        StartCoroutine("StartFishCountdown");
        sprite.sprite = pondFish;
    }

    public IEnumerator StartFishCountdown()
    {
        yield return new WaitForSeconds(countdown);
        withinFishTime = false;
        currFishing = false;
        //sprite.sprite = pond;
        //StopCoroutine("StartFishTimer");
    }
}
