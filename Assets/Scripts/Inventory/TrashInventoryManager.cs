/******************************************************************************
 * Trash class for player interaction. Main heavy lifting for managing the data structure
 * for Trash inventory. Inherits from PlayerItemManager.
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TrashInventoryManager : PlayerItemManager
{
    [SerializeField]
    private PlayerInventory playerInventory = null;

    //[SerializeField]
    //private Player2Behavior player = null;

    //public bool inventoryShiftClick = false;

    // do nothing
    public override void SplitStack(int slotIndex)
    {
        Debug.Log("Attempting to split equipment inventory - this method should not be called");
    }

    public void SwapWInventory(int inventoryIndex, int trashIndex)
    {
        ItemSlot trashSlot = inventory[trashIndex];
        ItemSlot inventorySlot = playerInventory.GetSlotByIndex(inventoryIndex);

        // if dropping item onto another item
        if (trashSlot.item != null)
        {
            // are they the same item?
            if (trashSlot.item == inventorySlot.item)
            {
                // check how much space is left inside slot
                int secondSlotRemainingSpace = trashSlot.GetMaxStack() - trashSlot.GetCurrStack();

                // if the slot can fit within the other slot, combine stacks
                if (inventorySlot.GetCurrStack() <= secondSlotRemainingSpace)
                {
                    // combine stacks
                    trashSlot.AddToStack(inventorySlot.GetCurrStack());

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
            //craftingStringForm.Remove(inventory[craftingIndex].GetItemName());
        }
        else
        {
            // the slot is being swapped with an empty slot
            // update with new size
            currInventorySize++;
            // tell inventory it has one less item now
            playerInventory.UpdateInventory(1);
        }

        // swap the slots 
        inventory[trashIndex] = inventorySlot;
        // update player inventory
        playerInventory.AddSlotByRef(trashSlot, inventoryIndex);

        OnItemsUpdated.Invoke();

    }
   
    public void TrashInventory()
	{
        for (int i = 0; i < maxInventorySize; i++)
        {
            DeleteFromInventory(i);
        }
        currInventorySize = 0;

    }

    // do nothing
    //public override void Swap(int indexOne, int indexTwo)
    //{
    //    Debug.Log("Attempting to drag equipment into wrong slots");
    //}

    // combine these two methods into one helper call - called in equip slot and inventory slot

    private void Awake()
    {
        // intialize the inventory with item slots based on current max inventory size
        for (int i = 0; i < maxInventorySize; i++)
        {
            inventory.Add(new ItemSlot());
        }
        OnItemsUpdated.Invoke();
    }

}
