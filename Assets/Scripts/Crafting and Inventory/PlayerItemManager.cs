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
    [SerializeField] private VoidEvent onEquipUpdated = null;

    // num of different stacked items it can hold
    [SerializeField]
    protected int maxInventorySize = 0;
    // num of different stacked items it is holding
    [SerializeField]
    protected int currInventorySize = 0;

    // inventory list
    protected List<ItemSlot> inventory = new();

    // fetches the inventory slot by index
    public ItemSlot GetSlotByIndex(int index) => inventory[index];

    public Action OnItemsUpdated = delegate { };
    public Action OnEquipUpdated = delegate { };

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

    public virtual void OnEnable()
    {
        OnItemsUpdated += onInventoryItemsUpdated.Raise;
        OnEquipUpdated += onEquipUpdated.Raise;
    }

    public virtual void OnDisable()
    {
        OnItemsUpdated -= onInventoryItemsUpdated.Raise;
        OnEquipUpdated -= onEquipUpdated.Raise;
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
                foundEmptySlot.SetItemBehavior(slot.itemBehavior);
                // account for new inventory size
                currInventorySize++;
                OnItemsUpdated.Invoke();
                // override this in crafting so that it updates the string form
            }
        }
    }

     //*IMPORTANT* REFACTOR THIS CODE LOTS OF REPEAT CODE HERE
    public virtual bool AddStack(ItemSlot itemSlot)
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
                foundEmptySlot.SetItemBehavior(itemSlot.itemBehavior);
                // account for new inventory size
                currInventorySize++;
                OnItemsUpdated.Invoke();
                // override this in crafting so that it updates the string form
                return true;
            }
            // inventory full after adding part of the stack
            return false;
        }
        return false;
    }

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

        // if not swap the slots
        inventory[indexOne] = secondSlot;
        //secondSlot.SetSlotIndex(indexOne);
        inventory[indexTwo] = firstSlot;
        //firstSlot.SetSlotIndex(indexTwo);

        OnItemsUpdated.Invoke();
    }

    public virtual void DeleteFromInventory(int slotIndex)
    {
        // if slotIndex passed in is out of bounds
        if (slotIndex < 0 || slotIndex > inventory.Count - 1) { return; }

        // delete object
        inventory[slotIndex] = new ItemSlot();
        //inventory[slotIndex].SetSlotIndex(slotIndex);
        currInventorySize--;

        OnItemsUpdated.Invoke();
    }

    public virtual void AddSlotByRef(ItemSlot itemSlot, int index)
    {
        inventory[index] = itemSlot;
        //itemSlot.SetSlotIndex(index);
        if (itemSlot.item != null)
        {
            //currInventorySize++;
        }
        OnItemsUpdated.Invoke();
    }

    public virtual void UpdateInventory()
    {
        currInventorySize--;
    }
}


public class ItemSlot
{
    // type of item in inventory slot
    public ItemData item;

    // specific item behavior info for what slot is curr holding
    public ItemBehavior itemBehavior;

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
        if (item != null)
        {
            return item.GetName();
        }
        else
        {
            return "";
        }
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
        if(currStack <= 0)
        {
            isEmpty = true;
            itemBehavior = null;
            //item = null;
        }
        else
        {
            isEmpty = false;
        }
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

    public void SetItemBehavior(ItemBehavior itemB)
    {
        itemBehavior = itemB;
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

    // add num to current stack
    public void AddToStack(int num)
    {
        currStack += num;
        if (currStack >= maxStack)
        {
            isFull = true;
        }
        isEmpty = false;
    }

    // subtract num from current stack
    public void SubFromStack(int num)
    {
        currStack -= num;
        if(currStack <= 0)
        {
            isEmpty = true;
            item = null;
            itemBehavior = null;
        }
    }

    //// update durability which is stored in item behavior
    //public void UpdateDurability()
    //{
    //    durability = itemBehavior.GetDurability();
    //    if(durability <= 0)
    //    {
    //        isEmpty = true;
    //        item = null;
    //        itemBehavior = null;
    //    }
    //}

    public int GetDurability()
    {
        int durability = itemBehavior.GetDurability();
        if (durability <= 0)
        {
            isEmpty = true;
            item = null;
            itemBehavior = null;
        }
        //Debug.Log("Durability: " + durability);
        return durability;
    }

    // return the amount of current stack halved
    public int HalveStack()
    {
        // rounded up
        double half = 2;
        double leftover = Math.Ceiling((double)currStack / half);

        // rounded down
        if (currStack > 1)
        {
            currStack /= 2;
        }
        else
        {
            return 0;
        }

        return (int)leftover;

    }
}
