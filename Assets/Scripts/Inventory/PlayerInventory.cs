/******************************************************************************
 * Inventory class for players.
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/

using DapperDino.Events.CustomEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// consider making inventory a scriptable object for save + load
public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private VoidEvent onInventoryItemsUpdated = null;
    // num of different stacked items it can hold
    [SerializeField]
    private int maxInventorySize = 40;
    private int currInventorySize = 0;

    //[SerializeField]

    // inventory list
    private List<ItemSlot> inventory = new();

    // possibly use an array for inventory
    // private StackObject[] inventory = new StackObject[maxInventorySize];

    private bool inventoryFull = false;

    // fetches the inventory slot by index
    public ItemSlot GetSlotByIndex(int index) => inventory[index];

    public Action OnItemsUpdated = delegate { };

    // add item to player inventory
    public bool AddItem(ItemData nItem)
    {
        Debug.Log("Attempting to add item to inventory, inventory count now: " + currInventorySize);
        // check for existing stacks with same item
        var foundStack = inventory.FirstOrDefault(stackItem => stackItem.item == nItem);
        // if found
        if (foundStack != null)
        {
            // if not full
            if (!(foundStack.IsFullStack()))
            {
                Debug.Log("Found existing stack that is not full, adding to it");
                // add item and return
                foundStack.AddToStack(nItem.GetCount());
                //foundStack.SetCurrStack(foundStack.GetCurrStack() + nItem.GetCount());
                OnItemsUpdated.Invoke();
                Debug.Log("Successfully added item, inventory count now: " + currInventorySize);
                Debug.Log("Stack of " + inventory[foundStack.GetSlotIndex()].GetItemName() + ": " + inventory[foundStack.GetSlotIndex()].GetCurrStack());
                return true;
            }
        }
        // if item is not currently in list or stack was full
        // add to inventory if inventory is not full
        if (currInventorySize < maxInventorySize)
        {
            // look for first empty inventory slot
            Debug.Log("Inventory is not full, looking for empty slot in inventory");
            var foundEmptySlot = inventory.Find(stackItem => stackItem.IsEmptySlot() == true);
            if (foundEmptySlot != null)
            {
                // change first empty inventory slot to hold item picked up
                foundEmptySlot.SetItemSlot(nItem, nItem.GetCount());
                // tell slot what it's index is in the array
                foundEmptySlot.SetSlotIndex(inventory.IndexOf(foundEmptySlot));
                // account for new inventory size
                currInventorySize++;
                OnItemsUpdated.Invoke();
                Debug.Log("Successfully added item, inventory count now: " + currInventorySize);
                Debug.Log("Stack of " + inventory[foundEmptySlot.GetSlotIndex()].GetItemName() + ": " + inventory[foundEmptySlot.GetSlotIndex()].GetCurrStack());
                return true;
            }
        }
        // else there is no space in inventory
        Debug.Log("Inventory full :(");
        inventoryFull = true;
        return false;
    }

    // getter method
    public bool IsInventoryFull()
    {
        return inventoryFull;
        // for if inventory full isn't detected in time
        // fail safe is checking every time this method is called
        /*if (inventory.Count >= maxInventorySize)
        {
           return true;
        }
        else 
           return false; */
    }

    //public int GetTotalQuantity(InventoryItem item)
    //{
    //    int totalCount = 0;

    //    foreach(StackObject stackObject in inventory)
    //    {
    //        totalCount += item
    //    }
    //}

    public void Swap(int indexOne, int indexTwo)
    {
        ItemSlot firstSlot = inventory[indexOne];
        ItemSlot secondSlot = inventory[indexTwo];

        // to be changed, need to override == operator to compare the two itemslot items
        // intent is to not do anything if the slot its being dropped on is it's own slot
        if(firstSlot == secondSlot)
        {
            return;
        }

        // if dropping item onto another item
        if(secondSlot.item != null)
        {
            // are they the same item?
            if(firstSlot.item == secondSlot.item)
            {
                // check how much space is left inside slot
                int secondSlotRemainingSpace = secondSlot.GetMaxStack() - secondSlot.GetCurrStack();

                // if the first slot can fit within the second slot, combine stacks
                if(firstSlot.GetCurrStack() <= secondSlotRemainingSpace)
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

    // init inventory before anything
    private void Awake()
    {
        // intialize the inventory with item slots based on current max inventory size
        for(int i = 0; i < maxInventorySize; i++)
        {
            inventory.Add(new ItemSlot());
        }
    }



    public void OnEnable() => OnItemsUpdated += onInventoryItemsUpdated.Raise;

    public void OnDisable() => OnItemsUpdated -= onInventoryItemsUpdated.Raise;

    //[ContextMenu("Test Add")]
    //public void TestAdd()
    //{
    //    AddItem(testItemSlot);
    //}

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