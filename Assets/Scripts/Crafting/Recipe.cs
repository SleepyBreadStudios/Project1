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

[CreateAssetMenu]
public class Recipe : ScriptableObject
{
    //[SerializeField]
    //private CraftingInventoryManager playerCrafting = null;

    //private int totalItems = 0;
    //public List<ItemData> items;

    //// Gets the items from the crafting slots
    //public void GetItems() {
    //    ItemSlot craftingItem = null;
    //    for (int i = 0; i < maxInventorySize; i++) {
    //        craftingItem = inventory[i];
    //        if (craftingItem.item != null) {
    //            Debug.Log(craftingItem.item);
    //            totalItems++;
    //            items.Add(craftingItem.item);
    //        }
    //    }
    //}

    //// Creates the item for the player and updates respective inventory and crafting slots
    //// Once an item is crafted, should be sent in the crafted slot
    //// Checks item present and compares across recipes
    //public void Craft() {
    //    if (items.Count == 0) {
    //        Debug.Log("No items present in crafting slots");
    //        return;
    //    }

    //}

    //public bool MakeDiamond() {

    //}

    //public enum CraftableItems {
    //    None,
    //    Diamond,
    //    Pentagon,
    //    Hexagon,
    //    Octagon
    //}
    [SerializeField]
    private string recipeContents = "DefaultString";

    [SerializeField]
    public List<string> recipeList = new();

    [SerializeField]
    private ItemData craftedItem = null;

    [SerializeField]
    private int numOfItems = 0;

    public bool IsRecipeEqual(List<string> craftingInput)
    {
        // num of items in the recipe have to be equal to 
        // num of items in the crafting
        if(craftingInput.Count != numOfItems)
        {
            return false;
        }
        for(int i = 0; i < craftingInput.Count; i++)
        {
            // check if item is used in this recipe
            if(!(recipeContents.Contains(craftingInput[i])))
            {
                // if one thing doesn't match it's already not equal
                return false;
            }
        }
        // if all items were found in the recipe
        // the recipe is true
        return true;
    }

    public ItemData GetCraftedItem()
    {
        return craftedItem;
    }

    public int GetNumItems()
    {
        return numOfItems;
    }
}
