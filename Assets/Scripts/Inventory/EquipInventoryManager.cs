/******************************************************************************
 * Crafting class for player interaction. Main heavy lifting for managing the data structure
 * for Equipment inventory. Inherits from PlayerItemManager.
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EquipInventoryManager : PlayerItemManager
{
    [SerializeField]
    private PlayerInventory playerInventory = null;

    //public bool inventoryShiftClick = false;

    private int headIndex = 0;
    private int chestIndex = 1;
    private int legIndex = 2;
    private int accIndex = 3;
    // do nothing
    public override void SplitStack(int slotIndex)
    {
        Debug.Log("Attempting to split equipment inventory - this method should not be called");
    }

    public bool AddStack(ItemSlot itemSlot, int slotIndex)
    {
        string equipType = itemSlot.item.equipType;
        // if it is not an equipment don't move into equipment
        if(equipType == null || equipType == "")
        {
#if Debug
            Debug.Log("Attempting to move item into equip when not an equip item");
#endif
            return false;
        }
        int count = itemSlot.GetCurrStack();
        switch (equipType)
        {
            case "Head":
                Debug.Log("Head equip");
                // if not already wearing head equip
                if (inventory[headIndex].item == null)
                {
                    //var headSlot = inventory[headIndex];
                    //headSlot.SetItemSlot(itemSlot.item, count);
                    //// tell slot what it's index is in the array
                    //headSlot.SetSlotIndex(headIndex);
                    //headSlot.SetItemBehavior(itemSlot.itemBehavior);
                    inventory[headIndex] = itemSlot;
                    // account for new inventory size
                    currInventorySize++;
                    OnItemsUpdated.Invoke();
                    return true;
                }
                else
                {
                    // swap the slots 
                    // update player inventory
                    var originalSlot = inventory[headIndex];
                    playerInventory.AddSlotByRef(originalSlot, slotIndex);
                    inventory[headIndex] = itemSlot;
                    OnItemsUpdated.Invoke();
                    // not because it failed but because we don't want to delete the item
                    // after we add it to inventory
                    return false;
                }
            case "Chest":
                Debug.Log("Chest equip");
                if (inventory[chestIndex].item == null)
                {
                    var chestSlot = inventory[chestIndex];
                    chestSlot.SetItemSlot(itemSlot.item, count);
                    // tell slot what it's index is in the array
                    chestSlot.SetSlotIndex(chestIndex);
                    chestSlot.SetItemBehavior(itemSlot.itemBehavior);
                    // account for new inventory size
                    currInventorySize++;
                    OnItemsUpdated.Invoke();
                    return true;
                }
                else
                {
                    // swap the slots 
                    // update player inventory
                    var originalSlot = inventory[chestIndex];
                    playerInventory.AddSlotByRef(originalSlot, slotIndex);
                    inventory[chestIndex] = itemSlot;
                    OnItemsUpdated.Invoke();
                    // not because it failed but because we don't want to delete the item
                    // after we add it to inventory
                    return false;
                }
            case "Leg":
                Debug.Log("Leg equip");
                if (inventory[legIndex].item == null)
                {
                    var legSlot = inventory[legIndex];
                    legSlot.SetItemSlot(itemSlot.item, count);
                    // tell slot what it's index is in the array
                    legSlot.SetSlotIndex(legIndex);
                    legSlot.SetItemBehavior(itemSlot.itemBehavior);
                    // account for new inventory size
                    currInventorySize++;
                    OnItemsUpdated.Invoke();
                    return true;
                }
                else
                {
                    // swap the slots 
                    // update player inventory
                    var originalSlot = inventory[legIndex];
                    playerInventory.AddSlotByRef(originalSlot, slotIndex);
                    inventory[legIndex] = itemSlot;
                    OnItemsUpdated.Invoke();
                    // not because it failed but because we don't want to delete the item
                    // after we add it to inventory
                    return false;
                }
            case "Accessory":
                Debug.Log("Accessory equip");
                break;
            default:
                Debug.Log("This shouldn't be called " + equipType);
                break;
        }
        return false;
    }

    public void SwapWInventory(int inventoryIndex, int equipIndex)
    {
        ItemSlot equipSlot = inventory[equipIndex];
        ItemSlot inventorySlot = playerInventory.GetSlotByIndex(inventoryIndex);

        string equipType = inventorySlot.item.equipType;
        if (equipType == null)
        {
            // if not an equipment don't move into equipment inventory
            return;
        }

        // if not dropping item onto another item
        if (equipSlot.item == null)
        {
            // the slot is being swapped with an empty slot
            // update with new size
            currInventorySize++;
            // tell inventory it has one less item now
            playerInventory.UpdateInventory();
        }

        if (equipIndex == headIndex)
        {
            if (equipType == "Head")
            {
                // swap the slots 
                inventory[equipIndex] = inventorySlot;
                // update player inventory
                playerInventory.AddSlotByRef(equipSlot, inventoryIndex);
            }
        }
        else if (equipIndex == chestIndex)
        {
            if (equipType == "Chest")
            {
                // swap the slots 
                inventory[equipIndex] = inventorySlot;
                // update player inventory
                playerInventory.AddSlotByRef(equipSlot, inventoryIndex);
            }
        }
        else if (equipIndex == legIndex)
        {
            if (equipType == "Leg")
            {
                // swap the slots 
                inventory[equipIndex] = inventorySlot;
                // update player inventory
                playerInventory.AddSlotByRef(equipSlot, inventoryIndex);
            }
        }
        else if (equipIndex == accIndex)
        {
            if (equipType == "Accessory")
            {
                // swap the slots 
                inventory[equipIndex] = inventorySlot;
                // update player inventory
                playerInventory.AddSlotByRef(equipSlot, inventoryIndex);
            }
        }

        OnItemsUpdated.Invoke();
    }

    // do nothing
    public override void Swap(int indexOne, int indexTwo)
    {
        Debug.Log("Attempting to drag equipment into wrong slots");
    }

    private void Awake()
    {
        // intialize the inventory with item slots based on current max inventory size
        for (int i = 0; i < maxInventorySize; i++)
        {
            inventory.Add(new ItemSlot());
        }
    }

}
