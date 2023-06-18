using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingUIManager : PlayerItemManager
{
    // Alicia: stuff i commented out, realizing this should prob not be the crafting manager but alas ;-;
    // Should replace this with inventory items later once we have it working
    //private Item currentItem;
    //public Image customCursor; 

    //public void OnMouseDownItem(Item item) {
    //    if (currentItem == null) {
    //        currentItem = item;
    //        //customCursor.gameObject.SetActive(true);
    //        //customeCursor.sprite = currentItem.GetComponent<Image>().sprite;
    //    }
    //}

    [SerializeField]
    private PlayerInventory playerInventory = null;

    public void SwapWInventory(int inventoryIndex, int craftingIndex)
    {
        Debug.Log("Hello");
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

        OnItemsUpdated.Invoke();
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