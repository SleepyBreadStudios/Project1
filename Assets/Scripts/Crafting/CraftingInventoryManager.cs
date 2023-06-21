/******************************************************************************
 * Crafting class for player interaction. Main heavy lifting for managing the data structure
 * for Crafting inventory. Inherits from PlayerItemManager. Intent is to handle
 * recipes and actual crafting in a separate script.
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//using System.Threading.Tasks;

public class CraftingInventoryManager : PlayerItemManager
{
    [SerializeField]
    private PlayerInventory playerInventory = null;

    [SerializeField]
    private CraftingManager craftingManager = null;

    //[SerializeField]
    //public ItemSlot resultSlot = null;

    [SerializeField]
    List<string> craftingStringForm = new();

    private int resultAmount = 0;

    private bool currentlyCrafting = false;

    public override bool IsInventoryFull()
    {
        //return inventoryFull;
        // for if inventory full isn't detected in time
        // fail safe is checking every time this method is called
        // account for result slot
        if (currInventorySize >= maxInventorySize - 1)
        {
            return true;
        }
        else
            return false;
    }

    public virtual void SplitStack(int slotIndex)
    {
        Debug.Log("Attempting to split stack");
        ItemSlot slot = inventory[slotIndex];
        // if can hold another stack
        if (!IsInventoryFull())
        {
            // halve the stack
            int newStackCount = slot.HalveStack();
            if (newStackCount == 0)
            {
                return;
            }
            // look for empty slot
            var foundEmptySlot = inventory.Find(stackItem => stackItem.IsEmptySlot() == true);
            if (foundEmptySlot != null)
            {
                // change first empty inventory slot to hold item picked up
                foundEmptySlot.SetItemSlot(slot.item, newStackCount);
                // tell slot what it's index is in the array
                foundEmptySlot.SetSlotIndex(inventory.IndexOf(foundEmptySlot));
                // account for new inventory size
                currInventorySize++;
                OnItemsUpdated.Invoke();
                craftingStringForm.Add(slot.GetItemName());
                AttemptToCraftItem();
                // override this in crafting so that it updates the string form
            }
        }
    }

    public override bool AddStack(ItemSlot itemSlot)
    {
        if (!IsInventoryFull())
        {
            int count = itemSlot.GetCurrStack();
            var foundStack = inventory.FirstOrDefault(stackItem => stackItem.item == itemSlot.item && stackItem.IsFullStack() != true);
            // if found
            if (foundStack != null)
            {
                // if not full
                if (!(foundStack.IsFullStack()))
                {
#if Debug
                Debug.Log("Found existing stack that is not full, adding to it");
#endif
                    // calculate remaining space in stack
                    int remainingSpace = foundStack.GetMaxStack() - foundStack.GetCurrStack();
                    // if it all fits in the stack, combine
                    if (count <= remainingSpace)
                    {
                        // add item and return
                        foundStack.AddToStack(count);
                        OnItemsUpdated.Invoke();
                        // don't need to add to crafting string if combining stacks
                        //craftingStringForm.Add(itemSlot.GetItemName());
                        AttemptToCraftItem();
#if Debug
                    Debug.Log("Successfully added item, inventory count now: " + currInventorySize);
                    Debug.Log("Stack of " + inventory[foundStack.GetSlotIndex()].GetItemName() + ": " + inventory[foundStack.GetSlotIndex()].GetCurrStack());
#endif
                        return true;
                    }
                    else
                    {
#if Debug
                    Debug.Log("Adding to existing stack, looking for another slot to add remaining");
#endif
                        // add what fits
                        foundStack.AddToStack(remainingSpace);
                        OnItemsUpdated.Invoke();
                        // then attempt to make a new stack with the rest
                        count -= remainingSpace;
                        itemSlot.SetCurrStack(count);
                    }
                }
            }
            // look for empty slot
            var foundEmptySlot = inventory.Find(stackItem => stackItem.IsEmptySlot() == true);
            if (foundEmptySlot != null)
            {
                // change first empty inventory slot to hold item picked up
                foundEmptySlot.SetItemSlot(itemSlot.item, count);
                // tell slot what it's index is in the array
                foundEmptySlot.SetSlotIndex(inventory.IndexOf(foundEmptySlot));
                // account for new inventory size
                currInventorySize++;
                OnItemsUpdated.Invoke();
                craftingStringForm.Add(itemSlot.GetItemName());
                AttemptToCraftItem();
                return true;
            }
            // inventory full after adding part of the stack
            AttemptToCraftItem();
            return false;
        }
        return false;
    }

    public void SwapWInventory(int inventoryIndex, int craftingIndex)
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
            // prepare for swap
            // remove old item name from crafting
            craftingStringForm.Remove(inventory[craftingIndex].GetItemName());
        }
        else
        {
            // the slot is being swapped with an empty slot
            // update with new size
            currInventorySize++;
            // tell inventory it has one less item now
            playerInventory.UpdateInventory();
        }

        // swap the slots 
        inventory[craftingIndex] = inventorySlot;
        // add to crafting string as well
        craftingStringForm.Add(inventorySlot.GetItemName());
        // update player inventory
        playerInventory.AddSlotByRef(craftingSlot, inventoryIndex);


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

    public void SwapWResult(int resultIndex, int craftingIndex)
    {
        ItemSlot craftingSlot = inventory[craftingIndex];
        ItemSlot resultSlot = inventory[resultIndex];

        // if dropping item onto another item
        // do nothing, should not swap with result slot
        if (craftingSlot.item != null)
        {
            return;
        }

        // if not swap the empty slot and full slot
        inventory[craftingIndex] = resultSlot;
        // update crafting based on successful crafted item
        Craft();

        // add to crafting string as well
        craftingStringForm.Add(resultSlot.GetItemName());
        currInventorySize++;

        // update result slot with empty slot
        inventory[resultIndex] = new ItemSlot();

        OnItemsUpdated.Invoke();
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

    // don't update inventory size when emptying result slot
    public void EmptyResultSlot()
    {
        inventory[maxInventorySize - 1] = new ItemSlot();
        OnItemsUpdated.Invoke();
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

    public void RemoveItemFromCrafting(int slotIndex)
    {
        craftingStringForm.Remove(inventory[slotIndex].GetItemName());
    }

    // every time crafting inventory is updated, check to see if we can make an item
    // updates preview of crafting
    public void AttemptToCraftItem()
    {
        // don't update crafting preview if there is crafting in the process
        if(currentlyCrafting)
        {
            return;
        }
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
            
            inventory[maxInventorySize - 1].SetItemSlot(recipe.GetCraftedItem(), lowestCount);
            resultAmount = lowestCount;
            OnItemsUpdated.Invoke();
        }
        else
        {
            // clear preview result if the new contents of the crafting inventory is not a recipe
            if(!(inventory[maxInventorySize - 1].IsEmptySlot()))
            {
                EmptyResultSlot();
                resultAmount = 0;
            }
        }
    }

    // Officially craft by updating item stacks to reflect that they've been used
    public void Craft()
    {
        currentlyCrafting = true;
        Recipe recipe = craftingManager.Craft(craftingStringForm);
        for (int i = 0; i < recipe.GetNumItems(); i++)
        {
            var foundItem = inventory.Find(itemSlot => itemSlot.GetItemName() == recipe.recipeList[i]);
            //Debug.Log(foundItem.GetItemName());
            if (foundItem != null)
            {
                //Debug.Log("Subtracting by " + resultAmount);
                foundItem.SetCurrStack(foundItem.GetCurrStack() - resultAmount);
                if (foundItem.IsEmptySlot())
                {
                    DeleteFromInventory(inventory.IndexOf(foundItem));
                }
            }
        }
        resultAmount = 0;
        AttemptToCraftItem();
        currentlyCrafting = false;
    }

    private void Awake()
    {
        // intialize the inventory with item slots based on current max inventory size
        for (int i = 0; i < maxInventorySize; i++)
        {
            inventory.Add(new ItemSlot());
        }
        //resultSlot = inventory[maxInventorySize - 1];
        craftingManager = GameObject.Find("CraftingManager").GetComponent<CraftingManager>();
    }
}