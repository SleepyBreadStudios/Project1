/******************************************************************************
 * Shared template for items.
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            Destroy(this.gameObject);
        }
    }
}
