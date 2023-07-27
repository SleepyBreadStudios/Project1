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

    // num to update sprite cross server
    private NetworkVariable<float> spriteNum = new NetworkVariable<float>();

    [SerializeField]
    private float spawnRadius = 2.0f;

    // how long it takes for bush to regenerate
    [SerializeField]
    private float countdown = 1.0f;

    private bool currFishing = false;

    private bool withinFishTime = false;

    private Player2Behavior currPlayer = null;

    public GameObject GetItem()
    {
        return item;
    }

    //void Start()
    //{
    //    sprite = GetComponent<SpriteRenderer>();
    //}

    void Update()
    {
        if (!currFishing)
        {
            StopCoroutine("StartFishTimer");
            StopCoroutine("StartFishCountdown");
            spriteNum.Value = 0;
        }

    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        sprite = gameObject.GetComponent<SpriteRenderer>();
        spriteNum.Value = 0;
        spriteNum.OnValueChanged += UpdateSprite;
    }


    public void Fish(Player2Behavior player)
    {
        if (withinFishTime)
        {
            if(player != currPlayer)
			{
                Debug.Log("This is a different player than who started the fishing!");
			}
            ItemDrop();
            currFishing = false;
            //sprite.sprite = pond;
            //StopCoroutine("StartFishCountdown");
            spriteNum.Value = 0;
            withinFishTime = false;
            currPlayer = null;
            return;
        }
        if (!currFishing)
        {
            if(currPlayer != player)
			{
                currPlayer = player;
            }                
            StartCoroutine("StartFishTimer");
            //sprite.sprite = pondCurrFish;
            spriteNum.Value = 1;
            currFishing = true;
        }
    }

    public void ItemDrop()
    {
        itemObj = Instantiate(item, new Vector3(Random.Range(transform.position.x - spawnRadius, transform.position.x + spawnRadius),
            Random.Range(transform.position.y - spawnRadius, transform.position.y + spawnRadius), -1), Quaternion.identity) as GameObject;
		itemObj.GetComponent<NetworkObject>().Spawn(true);
        spriteNum.Value = 0;
    }

	// Regenerating object
	public IEnumerator StartFishTimer()
    {
        yield return new WaitForSeconds(Random.Range(0.5f, 3f));
        withinFishTime = true;
        Debug.Log("Counting for fish");
        StartCoroutine("StartFishCountdown");
        spriteNum.Value = 2;
        //sprite.sprite = pondFish;
    }

    public IEnumerator StartFishCountdown()
    {
        yield return new WaitForSeconds(countdown);
        withinFishTime = false;
        currFishing = false;
        //spriteNum.Value = 0;
        //sprite.sprite = pond;
        //StopCoroutine("StartFishTimer");
    }

    public void UpdateSprite(float oldValue, float newValue)
	{
        if(newValue == 0)
		{
            sprite.sprite = pond;
        }
        else if (newValue == 1)
		{
            sprite.sprite = pondCurrFish;
        }
        else if(newValue == 2)
		{
            sprite.sprite = pondFish;
		}
	}
}
