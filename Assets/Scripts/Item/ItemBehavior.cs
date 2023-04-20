/******************************************************************************
 * Shared template for items. Handles it's own deletion.
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//#define Debug

// consider making abstract
public class ItemBehavior : MonoBehaviour
{
    public ItemData itemType;

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
    public void Delete()
    {
        Destroy(this.gameObject);
    }
    private void Start()
    {

        itemCount = itemType.GetCount();
        Debug.Log(itemCount);
    }
}
