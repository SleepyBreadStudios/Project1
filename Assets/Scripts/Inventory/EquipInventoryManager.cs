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

    [SerializeField]
    private Player2Behavior player = null;

    //public bool inventoryShiftClick = false;

    private int headIndex = 0;
    private int chestIndex = 1;
    private int legIndex = 2;
    private int accIndex = 3;

    [SerializeField]
    private List<ItemData> FrostItems = null;
    // do nothing
    public override void SplitStack(int slotIndex)
    {
        Debug.Log("Attempting to split equipment inventory - this method should not be called");
    }

    public bool AddStack(ItemSlot itemSlot, int slotIndex)
    {
        string equipType = itemSlot.item.GetEquipType();
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
                    inventory[headIndex] = itemSlot;
                    // account for new inventory size
                    currInventorySize++;
                    OnItemsUpdated.Invoke();
                    OnEquipUpdated.Invoke();
                    CheckResistCold();
                    CalculateCurrDef();
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
                    OnEquipUpdated.Invoke();
                    CheckResistCold();
                    CalculateCurrDef();
                    // not because it failed but because we don't want to delete the item
                    // after we add it to inventory
                    return false;
                }
            case "Chest":
                Debug.Log("Chest equip");
                if (inventory[chestIndex].item == null)
                {
                    inventory[chestIndex] = itemSlot;
                    // account for new inventory size
                    currInventorySize++;
                    OnItemsUpdated.Invoke();
                    OnEquipUpdated.Invoke();
                    CheckResistCold();
                    CalculateCurrDef();
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
                    OnEquipUpdated.Invoke();
                    CheckResistCold();
                    CalculateCurrDef();
                    // not because it failed but because we don't want to delete the item
                    // after we add it to inventory
                    return false;
                }
            case "Leg":
                Debug.Log("Leg equip");
                if (inventory[legIndex].item == null)
                {
                    inventory[legIndex] = itemSlot;
                    // account for new inventory size
                    currInventorySize++;
                    OnItemsUpdated.Invoke();
                    OnEquipUpdated.Invoke();
                    CheckResistCold();
                    CalculateCurrDef();
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
                    OnEquipUpdated.Invoke();
                    CheckResistCold();
                    CalculateCurrDef();
                    // not because it failed but because we don't want to delete the item
                    // after we add it to inventory
                    return false;
                }
            case "Accessory":
                Debug.Log("Accessory equip");
                if (inventory[accIndex].item == null)
                {
                    inventory[accIndex] = itemSlot;
                    // account for new inventory size
                    currInventorySize++;
                    OnItemsUpdated.Invoke();
                    OnEquipUpdated.Invoke();
                    CheckResistCold();
                    CalculateCurrDef();
                    return true;
                }
                else
                {
                    // swap the slots 
                    // update player inventory
                    var originalSlot = inventory[accIndex];
                    playerInventory.AddSlotByRef(originalSlot, slotIndex);
                    inventory[accIndex] = itemSlot;
                    OnItemsUpdated.Invoke();
                    OnEquipUpdated.Invoke();
                    CheckResistCold();
                    CalculateCurrDef();
                    // not because it failed but because we don't want to delete the item
                    // after we add it to inventory
                    return false;
                }
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

        string equipType = inventorySlot.item.GetEquipType();
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
            playerInventory.UpdateInventory(1);
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
        CheckResistCold();
        CalculateCurrDef();
        OnItemsUpdated.Invoke();
        OnEquipUpdated.Invoke();
    }

    // do nothing
    public override void Swap(int indexOne, int indexTwo)
    {
        Debug.Log("Attempting to drag equipment into wrong slots");
    }

    // combine these two methods into one helper call - called in equip slot and inventory slot
    public void CalculateCurrDef()
    {
        float count = 0;
        for (int i = 0; i <= 3; i++)
        {
            if(inventory[i].item != null)
            {
                count += inventory[i].item.GetDefense();
            }
        }
        player.UpdateDefense(count);
    }

    public bool CheckResistCold()
	{
        //Debug.Log("hello");
        // check if have accessory on
        Debug.Log("Resisting cold?");
        var tempSize = currInventorySize;
        if(!inventory[3].IsEmptySlot())
		{
            tempSize--;
            Debug.Log("Accessory worn, subtracting from total");
		}
        if(tempSize != FrostItems.Count)
		{
            Debug.Log("Inventory size: " + tempSize);
            Debug.Log("Frost gear size: " + FrostItems.Count);
            player.ResistCold(false);
            // less than what is required
            return false;
		}
        Debug.Log("1 Inventory size: " + tempSize);
        Debug.Log("2 Inventory size: " + currInventorySize);
        for (int i = 0; i < FrostItems.Count; i++)
        {
            if (inventory[i].item != null)
            {
                if(!FrostItems.Contains(inventory[i].item))
				{
                    // if don't have one of the set, no resistance
                    player.ResistCold(false);
                    return false;
				}
                //count += inventory[i].item.GetDefense();  
            }
            else
			{
                // if any are null then set not complete and no resistance
                player.ResistCold(false);
                return false;
			}
        }
        Debug.Log("Resist cold = true");
        player.ResistCold(true);
        return true;
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
