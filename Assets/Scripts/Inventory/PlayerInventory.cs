/******************************************************************************
 * Inventory class for players. Main heavy lifting for managing the data structure
 * for player inventory. Inherits from PlayerItemManager.
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/
//#define Debug
using DapperDino.Events.CustomEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// consider making inventory a scriptable object for save + load
public class PlayerInventory : PlayerItemManager
{
    [SerializeField] private VoidEvent onHotbarUpdated = null;

    [SerializeField]
    private CraftingInventoryManager playerCrafting = null;
    //[SerializeField] private VoidEvent onInventoryItemsUpdated = null;
    // num of different stacked items it can hold

    //[SerializeField]

    // inventory list
    private List<ItemSlot> hotbar = new();

    // possibly use an array for inventory
    // private StackObject[] inventory = new StackObject[maxInventorySize];

    //private bool inventoryFull = false;

    // fetches the inventory slot by index
    //public ItemSlot GetSlotByIndex(int index) => inventory[index];

    public Action OnHotbarUpdated = delegate { };

    //public void OnEnable() => OnHotbarUpdated += onHotbarUpdated.Raise;

    public int currHotbarNum = 1;

    public bool inventoryShiftClick = false;

    // add item to player inventory
    public bool AddItem(ItemBehavior nBehavior, ItemData nItem)
    {
        // temporary count variable for changing 
        int count = nBehavior.GetCount();
#if Debug
        Debug.Log("Attempting to add item to inventory, inventory count now: " + currInventorySize);
#endif
        // check if item is unique
        if(nItem.GetUnique())
        {
            // if it is, check if we have the item in the inventory yet
            var foundUnique = inventory.Find(stackItem => stackItem.item == nItem);
            // already have item? return
            if(foundUnique != null)
            {
                return false;
            }
        }
        // check for existing not full stacks with same item 
        var foundStack = inventory.FirstOrDefault(stackItem => stackItem.item == nItem && stackItem.IsFullStack() != true);
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
                    nBehavior.SetCount(count);
                }
            }
        }
        // if item is not currently in list or stack was full
        // add to inventory if inventory is not full
        if (currInventorySize < maxInventorySize)
        {
            // look for first empty inventory slot
#if Debug
            Debug.Log("Inventory is not full, looking for empty slot in inventory");
#endif
            var foundEmptySlot = inventory.Find(stackItem => stackItem.IsEmptySlot() == true);
            if (foundEmptySlot != null)
            {
                // change first empty inventory slot to hold item picked up
                foundEmptySlot.SetItemSlot(nItem, count);
                // tell slot what it's index is in the array
                foundEmptySlot.SetSlotIndex(inventory.IndexOf(foundEmptySlot));
                // account for new inventory size
                currInventorySize++;
                OnItemsUpdated.Invoke();
#if Debug
                Debug.Log("Successfully added item, inventory count now: " + currInventorySize);
                Debug.Log("Stack of " + inventory[foundEmptySlot.GetSlotIndex()].GetItemName() + ": " + inventory[foundEmptySlot.GetSlotIndex()].GetCurrStack());
#endif
                return true;
            }
        }
        // else there is no space in inventory
#if Debug
        Debug.Log("Inventory full");
#endif
        //inventoryFull = true;
        return false;
    }

    public void SwapWCrafting(int craftingIndex, int inventoryIndex)
    {
        ItemSlot inventorySlot = inventory[inventoryIndex];
        ItemSlot craftingSlot = playerCrafting.GetSlotByIndex(craftingIndex);

        // if dropping item onto another item
        if (inventorySlot.item != null)
        {
            // are they the same item?
            if (craftingSlot.item == inventorySlot.item)
            {
                // check how much space is left inside slot
                int secondSlotRemainingSpace = inventorySlot.GetMaxStack() - inventorySlot.GetCurrStack();

                // if the slot can fit within the other slot, combine stacks
                if (craftingSlot.GetCurrStack() <= secondSlotRemainingSpace)
                {
                    // combine stacks
                    inventorySlot.AddToStack(craftingSlot.GetCurrStack());

                    // set inventory slot to an empty slot
                    // same as deleting basically
                    playerCrafting.DeleteFromInventory(craftingIndex);

                    // not sure if we need this line but it's intent is to delete the slot we aren't using anymore
                    inventorySlot = null;

                    OnItemsUpdated.Invoke();

                    return;
                }
            }
        }
        else
        {
            // the slot is being swapped with an empty slot
            // update with new size
            currInventorySize++;
            // tell crafting it has one less item now
            playerCrafting.UpdateInventory();
        }

        // if not swap slots
        inventory[inventoryIndex] = craftingSlot;
        playerCrafting.RemoveItemFromCrafting(craftingIndex);
        playerCrafting.AddSlotByRef(inventorySlot, craftingIndex);
       

        

        OnItemsUpdated.Invoke();
        // since swapping with crafting, need to attempt to craft
        playerCrafting.AttemptToCraftItem();
    }

    public void SwapWResult(int resultIndex, int inventoryIndex)
    {
        ItemSlot inventorySlot = inventory[inventoryIndex];
        ItemSlot craftingResultSlot = playerCrafting.GetSlotByIndex(resultIndex);

        // if dropping item onto another item
        // do nothing, should not swap with result slot
        if (inventorySlot.item != null)
        {
            return;
        }

        // if not swap the empty slot and full slot
        inventory[inventoryIndex] = craftingResultSlot;
        currInventorySize++;

        // update crafting based on successful crafted item
        playerCrafting.Craft();
        // then update result slot to be empty
        playerCrafting.AddSlotByRef(inventorySlot, resultIndex);

        OnItemsUpdated.Invoke();
    }

    public void useHotbarItem(int currHotbarSelected)
    {
        // adjust for array starting at 0 
        if (currHotbarSelected == 0)
        {
            currHotbarSelected = 9;
        }
        else
        {
            currHotbarSelected--;
        }

        if(inventory[currHotbarSelected].item == null)
        {
            Debug.Log("Hotbar slot empty");
            return;
        }
        else
        {
            // this is where we trigger the actual effect of the item
            Debug.Log("Do something here");
        }
    }

    // for updating the selection sprite on the hotbar ui
    // called by player behavior
    public void updateHotbar(int currHotbarSelected)
    {
        currHotbarNum = currHotbarSelected;
        OnHotbarUpdated.Invoke();
    }

    // called by player behavior
    public void inventoryTransferEnabled(bool enable)
    {
        inventoryShiftClick = enable;
    }

    // init inventory before anything
    private void Awake()
    {
        OnHotbarUpdated += onHotbarUpdated.Raise;
        // intialize the inventory with item slots based on current max inventory size
        for (int i = 0; i < maxInventorySize; i++)
        {
            inventory.Add(new ItemSlot());
        }
        for(int i = 0; i < 11; i++)
        {
            hotbar.Add(inventory[i]);
        }
    }
}
