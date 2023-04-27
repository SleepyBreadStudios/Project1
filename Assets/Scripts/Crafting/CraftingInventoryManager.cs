/******************************************************************************
 * Crafting class for player interaction. Main heavy lifting for managing the data structure
 * for Crafting inventory. Inherits from PlayerItemManager. Intent is to handle
 * recipes and actual crafting in a separate script.
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingInventoryManager : PlayerItemManager
{
    [SerializeField]
    private PlayerInventory playerInventory = null;

    [SerializeField]
    private CraftingManager craftingManager = null;

    [SerializeField]
    public ItemSlot resultSlot = null;

    List<string> craftingStringForm = new();



    public void SwapCrafting(int inventoryIndex, int craftingIndex)
    {
        ItemSlot craftingSlot = inventory[craftingIndex];
        ItemSlot inventorySlot = playerInventory.GetSlotByIndex(inventoryIndex);

        // if dropping item onto another item
        if (craftingSlot.item != null)
        { 
            // are they the same item?
            if (craftingSlot.item == inventorySlot.item)
            {
                // check how much space is left inside slot
                int secondSlotRemainingSpace = craftingSlot.GetMaxStack() - craftingSlot.GetCurrStack();

                // if the slot can fit within the other slot, combine stacks
                if (inventorySlot.GetCurrStack() <= secondSlotRemainingSpace)
                {
                    // combine stacks
                    craftingSlot.AddToStack(inventorySlot.GetCurrStack());

                    // set inventory slot to an empty slot
                    // same as deleting basically
                    playerInventory.DeleteFromInventory(inventoryIndex);

                    // not sure if we need this line but it's intent is to delete the slot we aren't using anymore
                    inventorySlot = null;

                    OnItemsUpdated.Invoke();

                    return;
                }
            }
        }

        //// if not swap the empty slot and full slot
        inventory[craftingIndex] = inventorySlot;
        playerInventory.AddSlotByRef(craftingSlot, inventoryIndex);
        currInventorySize++;
        // add to crafting string as well
        craftingStringForm.Add(inventorySlot.GetItemName());

        OnItemsUpdated.Invoke();
        AttemptToCraftItem();
    }

    // no need to update crafting string because this is only called
    // between two items already in the crafting string
    public override void Swap(int indexOne, int indexTwo)
    {
        base.Swap(indexOne, indexTwo);
        AttemptToCraftItem();
    }

    public override void DeleteFromInventory(int slotIndex)
    {
        // if slotIndex passed in is out of bounds
        if (slotIndex < 0 || slotIndex > inventory.Count - 1) { return; }
        // update crafting string to reflect removal
        craftingStringForm.Remove(inventory[slotIndex].GetItemName());
        // delete as normal
        base.DeleteFromInventory(slotIndex);
        AttemptToCraftItem();
    }

    public override void AddSlotByRef(ItemSlot itemSlot, int index)
    {
        base.AddSlotByRef(itemSlot, index);
        // if it's not an empty item
        if(itemSlot.item != null)
        {
            craftingStringForm.Add(itemSlot.GetItemName());
        }

        AttemptToCraftItem();
    }

    // every time crafting inventory is updated, check to see if we can make an item
    public void AttemptToCraftItem()
    {
        Recipe recipe = craftingManager.Craft(craftingStringForm);
        if(recipe != null)
        {
            int lowestCount = 100;
            // have to check every slot due to being able to drag into any
            for(int i = 0; i < maxInventorySize; i++)
            {
                if(!(inventory[i].IsEmptySlot()))
                {
                    if(inventory[i].GetCurrStack() < lowestCount)
                        lowestCount = inventory[i].GetCurrStack();
                }
            }
            resultSlot.SetItemSlot(recipe.GetCraftedItem(), lowestCount);
            // update counts to reflect new item
            for(int i = 0; i < recipe.GetNumItems(); i++)
            {
                var foundItem = inventory.Find(itemSlot => itemSlot.GetItemName() == recipe.recipeList[i]);
                foundItem.SetCurrStack(foundItem.GetCurrStack() - lowestCount);
                if(foundItem.IsEmptySlot())
                {
                    DeleteFromInventory(inventory.IndexOf(foundItem));
                }
            }
        }
        OnItemsUpdated.Invoke();
    }

    private void Awake()
    {
        // intialize the inventory with item slots based on current max inventory size
        for (int i = 0; i < maxInventorySize; i++)
        {
            inventory.Add(new ItemSlot());
        }
        resultSlot = inventory[maxInventorySize - 1];
        craftingManager = GameObject.Find("CraftingManager").GetComponent<CraftingManager>();
    }
}
