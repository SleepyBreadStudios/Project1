/******************************************************************************
 * Recipe class used to store different recipes for the game. The crafting system should reference the
 * recipe class somehow and check the items in the crafting slot to see what item can be made. The number
 * of items and the type of items matter.
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recipe : CraftingInventoryManager
{

    [SerializeField]
    private CraftingInventoryManager playerCrafting = null;

    private int totalItems = 0;
    public List<ItemData> items;

    // Gets the items from the crafting slots
    public void GetItems() {
        ItemSlot craftingItem = null;
        for (int i = 0; i < maxInventorySize; i++) {
            craftingItem = inventory[i];
            if (craftingItem.item != null) {
                Debug.Log(craftingItem.item);
                totalItems++;
                items.Add(craftingItem.item);
            }
        }
    }

    // Creates the item for the player and updates respective inventory and crafting slots
    // Once an item is crafted, should be sent in the crafted slot
    // Checks item present and compares across recipes
    public void Craft() {
        if (items.Count == 0) {
            Debug.Log("No items present in crafting slots");
            return;
        }

    }

    /*
     * Because I don't fully understand how items are currently set up and the video in part
     * goes over an implementation for items to link to the crafting implementation, this is
     * an implementation that made sense to me. Recipes are stored in an array of pairs with
     * size 5, representing the 5 crafting slots. Each pair is a itemtype/num pair, the num
     * representing which slot it should be in (in case we want order to matter). If the order
     * does not matter, set the number to -1 in which case the test function will check for
     * that item in every slot.
     */

    public enum CraftableItems {
        None,
        Diamond,
        Pentagon,
        Hexagon,
        Octagon,
        PurpCirc
    }
}
