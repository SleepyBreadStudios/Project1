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

    public bool MakeDiamond() {
        
    }

    public enum CraftableItems {
        None,
        Diamond,
        Pentagon,
        Hexagon,
        Octagon
    }
}
