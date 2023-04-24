/******************************************************************************
 * Shared template for items. Handles it's own deletion.
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/
//#define Debug
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
        
    }

    private void Update()
    {
        // Random movement of objects to test performance of code
        Vector3 pos = transform.position;
        int randx = Random.Range(0, 2);
        int randy = Random.Range(0, 2);
        if (randx == 0) {
            pos.x -= (float)0.01;
        } else {
            pos.x += (float)0.01;
        }

        if (randy == 0) {
            pos.y -= (float)0.001;
        } else {
            pos.y += (float)0.001;
        } 

        transform.position = pos;

        
    }
}
