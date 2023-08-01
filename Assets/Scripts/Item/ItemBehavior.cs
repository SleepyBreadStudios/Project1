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
public class ItemBehavior : NetworkBehaviour
{
    public ItemData itemType;
    private Vector2 randLoc;
    private float immunity = 0.0f;

    [SerializeField]
    protected string itemName;

    //private readonly string[] itemPrefabs = {"capsule", "checkmark", "circle", "diamond", "flower", "hexagonpoint", "roundedsquare", "star", "triangle"};

    // individual items can have their own count that alters based on players
    [SerializeField]
    private NetworkVariable<int> itemCount = new NetworkVariable<int>();

    private int itemDurability = 100;

    public string leftOrRight = "";

    [SerializeField]
    public bool toolSwing;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        itemCount.Value = itemType.GetCount();
        //itemCount.OnValueChanged += UpdateCount;
        itemDurability = itemType.GetStartingCondition();
        itemName = itemType.GetName();
    }

    protected virtual void Awake()
    {
        itemDurability = itemType.GetStartingCondition();
    }

    public ItemData GetItemType()
    {
        return itemType;
    }

    public int GetCount()
    {
        //Debug.Log("Get " + itemCount);
        return itemCount.Value;
    }

    public string GetItemName()
	{
        return itemName;
	}

    public bool IsTool()
	{
        return toolSwing;
	}

    public void SetCount(int newCount)
    {
        itemCount.Value = newCount;
        //Debug.Log(itemCount);
        GetCount();
    }

    public int GetDurability()
    {
        return itemDurability;
    }

    public void DepleteDurability(int num)
    {
        itemDurability -= num;
    }

    // on pick up delete
    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
#if Debug
        Debug.Log("Item collision detected");
#endif
        if(collision.CompareTag("Player"))
        {
            // if inventory is not full
            if(!(collision.gameObject.GetComponent<PlayerInventory>().IsInventoryFull()))
            {
#if Debug
                Debug.Log("Item picked up, destroying overworld object");
#endif
               Destroy(this.gameObject);
            }
#if Debug
            else
            {
                Debug.Log("Item was not picked up, player collided with's inventory is full");
            }
#endif
        }
    }
    */

    // on pick up delete, determined by player based on whether item was able to be picked up or not
 //   public void UpdateCount(float oldValue, newValue)
	//{
 //           if (itemCount.Value != newValue)
 //           {
 //               itemCount.Value = newValue;
 //           }
 //   }

    public void Delete()
    {
        DespawnServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void DespawnServerRpc()
	{
        GetComponent<NetworkObject>().Despawn(true);
	}

    public override void OnNetworkDespawn()
    {
        Destroy(this.gameObject);
    }

    public void Hide()
    {
        this.gameObject.GetComponent<Renderer>().enabled = false;
        this.gameObject.GetComponent<Collider2D>().enabled = false;
    }

    public void move()
    {
        float randy = Random.Range
                (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y);
        float randx = Random.Range
            (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x);

        randLoc = new Vector2(randx, randy);
    }

    protected virtual void Update()
    {
        // immunity timer up to determine whether object can get collision deleted
        immunity += Time.deltaTime;
        
        //transform.position = Vector3.MoveTowards(transform.position, randLoc, Time.deltaTime * (float)0.3);

    }

    //void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.gameObject.CompareTag("Item") && immunity > 10.0f)
    //    {
    //        string toLoad = "Items/" + itemPrefabs[Random.Range(0, 10)];
    //        GameObject newItem = Instantiate(Resources.Load("Projectile", typeof(GameObject)), transform.position, Quaternion.identity) as GameObject;
    //        this.Delete();
    //    }
    //}

    public virtual string GetItemEffect(Player2Behavior playerBehavior)
    {
        Debug.Log("Attempting to use item from hotbar. This item does nothing in hotbar");
        return "NoEffect";
    }
}
