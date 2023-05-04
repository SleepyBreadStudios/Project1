/******************************************************************************
 * Base abstract class for Crafting and Inventory. Refactoring and renaming needed.
 * Class handles the data structure for crafting and inventory since both are similar.
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

public abstract class PlayerItemManager : MonoBehaviour
{
    [SerializeField] private VoidEvent onInventoryItemsUpdated = null;
    // num of different stacked items it can hold
    [SerializeField]
    protected int maxInventorySize = 0;
    // num of different stacked items it is holding
    protected int currInventorySize = 0;

    // inventory list
    protected List<ItemSlot> inventory = new();

    // fetches the inventory slot by index
    public ItemSlot GetSlotByIndex(int index) => inventory[index];

    public Action OnItemsUpdated = delegate { };

    // getter method
    public virtual bool IsInventoryFull()
    {
        //return inventoryFull;
        // for if inventory full isn't detected in time
        // fail safe is checking every time this method is called
        if (currInventorySize >= maxInventorySize)
        {
            return true;
        }
        else
            return false;
    }

    public virtual void OnEnable() => OnItemsUpdated += onInventoryItemsUpdated.Raise;

    public virtual void OnDisable() => OnItemsUpdated -= onInventoryItemsUpdated.Raise;

    // this code is duplicated in it's children classes, refactoring required
    public virtual void Swap(int indexOne, int indexTwo)
    {
        ItemSlot firstSlot = inventory[indexOne];
        ItemSlot secondSlot = inventory[indexTwo];

        // to be changed, need to override == operator to compare the two itemslot items
        // intent is to not do anything if the slot its being dropped on is it's own slot
        if (firstSlot == secondSlot)
        {
            return;
        }

        // if dropping item onto another item
        if (secondSlot.item != null)
        {
            // are they the same item?
            if (firstSlot.item == secondSlot.item)
            {
                // check how much space is left inside slot
                int secondSlotRemainingSpace = secondSlot.GetMaxStack() - secondSlot.GetCurrStack();

                // if the first slot can fit within the second slot, combine stacks
                if (firstSlot.GetCurrStack() <= secondSlotRemainingSpace)
                {
                    // combine first stack into second stack
                    secondSlot.AddToStack(firstSlot.GetCurrStack());

                    // set first inventory slot to an empty slot
                    inventory[indexOne] = new ItemSlot();

                    // not sure if we need this line but it's intent is to delete the slot we aren't using anymore
                    firstSlot = null;

                    OnItemsUpdated.Invoke();

                    return;
                }
            }
        }

        // if not swap the empty slot and full slot
        inventory[indexOne] = secondSlot;
        inventory[indexTwo] = firstSlot;

        OnItemsUpdated.Invoke();
    }

    public virtual void DeleteFromInventory(int slotIndex)
    {
        // if slotIndex passed in is out of bounds
        if (slotIndex < 0 || slotIndex > inventory.Count - 1) { return; }

        // delete object
        inventory[slotIndex] = new ItemSlot();
        currInventorySize--;

        OnItemsUpdated.Invoke();
    }

    public virtual void AddSlotByRef(ItemSlot itemSlot, int index)
    {
        inventory[index] = itemSlot;
        OnItemsUpdated.Invoke();
    }
}


public class ItemSlot
{
    // type of item in inventory slot
    public ItemData item;

    // max num of items that stack can hold
    private int maxStack = 0;

    // curr num of items stack is holding
    [SerializeField]
    private int currStack = 0;

    private int slotIndex = -1;

    private bool isFull = false;
    private bool isEmpty = true;

    // constructor methods
    public ItemSlot()
    {
        item = null;
        maxStack = 0;
        currStack = 0;
        isEmpty = true;
    }

    public ItemSlot(ItemData itemData, int num)
    {
        item = itemData;
        maxStack = item.GetMaxStackAmount();
        currStack = num;
        isEmpty = false;
    }

    // getter methods
    public string GetItemName()
    {
        return item.GetName();
    }

    public int GetMaxStack()
    {
        return maxStack;
    }

    public int GetCurrStack()
    {
        return currStack;
    }

    public int GetSlotIndex()
    {
        return slotIndex;
    }

    // set methods
    // possibly obsolete method
    public void SetCurrStack(int newStack)
    {
        currStack = newStack;
        if (currStack >= maxStack)
        {
            isFull = true;
        }
        isEmpty = false;
    }

    public void SetItemSlot(ItemData itemData, int num)
    {
        item = itemData;
        maxStack = item.GetMaxStackAmount();
        currStack = num;
        isEmpty = false;
    }

    public void SetSlotIndex(int newSlotIndex)
    {
        slotIndex = newSlotIndex;
        isEmpty = false;
    }

    // is stack full
    public bool IsFullStack()
    {
        if (currStack >= maxStack)
        {
            isFull = true;
        }
        return isFull;
    }

    // is slot empty
    public bool IsEmptySlot()
    {
        return isEmpty;
    }

    public void AddToStack(int num)
    {
        currStack += num;
        if (currStack >= maxStack)
        {
            isFull = true;
        }
        isEmpty = false;
    }
}

