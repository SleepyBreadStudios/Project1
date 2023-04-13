/******************************************************************************
 * Inventory class for players.
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// consider making inventory a scriptable object for save + load
public class PlayerInventory : MonoBehaviour
{
    // num of different stacked items it can hold
    [SerializeField]
    private int numItems = 10;

    private List<StackObject> inventory = new();

    // add item to player inventory
    public bool AddItem(ItemData item) 
    {
        Debug.Log("Attempting to add item to inventory, inventory count now: " + inventory.Count);
        // check for existing stacks with same item
        var foundStack = inventory.FirstOrDefault(stackItem => stackItem.itemType == item);
        // if found
        if(foundStack != null)
        {
            // if not full
            if(!(foundStack.IsFullStack()))
            {
                // add item and return
                foundStack.SetCurrStack(foundStack.GetCurrStack() + item.GetCount());
                Debug.Log("Successfully added item, inventory count now: " + inventory.Count);
                Debug.Log("Stack of " + inventory[inventory.Count - 1].GetItemName() + ": " + inventory[inventory.Count - 1].GetCurrStack());
                return true;
            }
        }
        // if item is not currently in list or stack was full
        // add to inventory if inventory is not full
        if(inventory.Count < numItems)
        {
            // create a stack with the item and it's count we picked up
            inventory.Add(new StackObject(item, item.GetCount()));
            Debug.Log("Successfully added item, inventory count now: " + inventory.Count);
            Debug.Log("Stack of " + inventory[inventory.Count - 1].GetItemName() + ": " + inventory[inventory.Count - 1].GetCurrStack());
            return true;
        }
        // else there is no space in inventory
        Debug.Log("Inventory full :(");
        return false;
    }
}

public class StackObject 
{
    // type of item in stack
    public ItemData itemType;

    // max num of items that stack can hold
    [SerializeField]
    private int maxStack = 64;

    // curr num of items stack is holding
    [SerializeField]
    private int currStack = 1;

    private bool isFull = false;

    public StackObject(ItemData itemData, int num)
    {
        itemType = itemData;
        currStack = num;
    }

    public string GetItemName()
    {
        return itemType.GetName();
    }

    // getter methods
    public int GetMaxStack() 
    {
        return maxStack;
    } 

    public int GetCurrStack()
    {
        return currStack;
    }

    // is stack full
    public bool IsFullStack()
    {
        return isFull;
    }

    public void SetCurrStack(int newStack)
    {
        currStack = newStack;
        if(currStack == maxStack) 
        {
            isFull = true;
        }
    }
}