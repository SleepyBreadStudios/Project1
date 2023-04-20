/******************************************************************************
 * Shared template for items. Handles it's own deletion.
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// consider making abstract
public class ItemBehavior : MonoBehaviour
{
    public ItemData itemType;

    public ItemData GetItemType()
    {
        return itemType;
    }
    // on pick up delete
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Item collision detected");
        if(collision.CompareTag("Player"))
        {
            // if inventory is not full
            if(!(collision.gameObject.GetComponent<PlayerInventory>().IsInventoryFull()))
            {
               Debug.Log("Item picked up, destroying overworld object");
               Destroy(this.gameObject);
            }
            else
            {
               Debug.Log("Item was not picked up, player collided with's inventory is full");
            }
        }
    }
}
