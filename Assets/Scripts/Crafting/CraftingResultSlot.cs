/******************************************************************************
 * Crafting result slot class. Inherits from ItemSlotUI. Handles the result UI.
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/
//#define Debug
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CraftingResultSlot : ItemSlotUI, IDropHandler
{
    [SerializeField]
    private CraftingInventoryManager crafting = null;

    [SerializeField]
    private PlayerInventory inventory = null;

    [SerializeField]
    private TextMeshProUGUI itemQuantityText = null;

    public override ItemSlot SlotItem
    {
        get { return ItemSlot; }
        set { }
    }

    public ItemSlot ItemSlot => crafting.GetSlotByIndex(SlotIndex);

    // do nothing, don't want to be able to drag into crafting result spot
    public override void OnDrop(PointerEventData eventData)
    {
        ItemDragHandler itemDragHandler = eventData.pointerDrag.GetComponent<ItemDragHandler>();
        Debug.Log("Attempting to drop on crafting result space");
    }

    // do nothing, don't want to be able to split crafting result spot
    public override void SplitStack()
    {
        Debug.Log("Attempting to split result");
    }

    // move stack from result slot to inventory with shift click
    public override void QuickMoveStack()
    {
        if (inventory.inventoryShiftClick)
        {
            if (inventory.AddStack(ItemSlot))
            {
                // item successfully moved from crafting
                // empty crafting slot
    //            /var foundStack = inventory.FirstOrDefault(stackItem => stackItem == ItemSlot);
    //            if(foundStack != null)
				//{
    //                Debug.Log("Found item");
				//}
                crafting.Craft(ItemSlot.GetItemName(), ItemSlot.itemBehavior, ItemSlot);
                
                //crafting.DeleteFromInventory(SlotIndex);
                crafting.EmptyResultSlot();
                crafting.AttemptToCraftItem();
            }
        }
    }

    public override void UpdateSlotUI()
    {
        if (ItemSlot.item == null)
        {
            EnableSlotUI(false);
            return;
        }

        EnableSlotUI(true);

        itemIconImage.sprite = ItemSlot.item.icon;
        itemQuantityText.text = ItemSlot.GetCurrStack().ToString();
    }

    protected override void EnableSlotUI(bool enable)
    {
        base.EnableSlotUI(enable);
        itemQuantityText.enabled = enable;
    }

    //public void DragDelete(int slotIndex)
    //{
    //    inventory.DeleteFromInventory(slotIndex);
    //}

    private void Awake()
    {
        SlotType = "CraftingResultSlot";
    }
}
