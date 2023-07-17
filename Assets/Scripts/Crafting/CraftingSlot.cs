/******************************************************************************
 * Crafting slot class. Inherits from ItemSlotUI. Handles crafting UI.
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

public class CraftingSlot : ItemSlotUI, IDropHandler
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

    public override void OnDrop(PointerEventData eventData)
    {
        ItemDragHandler itemDragHandler = eventData.pointerDrag.GetComponent<ItemDragHandler>();
        Debug.Log("Attempting to place item in crafting");
        // swap stack positions
        if (itemDragHandler.ItemSlotUI.SlotType == "InventorySlot")
        {
            if ((itemDragHandler.ItemSlotUI as InventorySlot) != null)
            {
                crafting.SwapWInventory(itemDragHandler.ItemSlotUI.SlotIndex, SlotIndex);
            }
        }
        else if(itemDragHandler.ItemSlotUI.SlotType == "CraftingSlot")
        {
            if ((itemDragHandler.ItemSlotUI as CraftingSlot) != null)
            {
                crafting.Swap(itemDragHandler.ItemSlotUI.SlotIndex, SlotIndex);
            }
        }
        else if (itemDragHandler.ItemSlotUI.SlotType == "CraftingResultSlot")
        {
            if ((itemDragHandler.ItemSlotUI as CraftingResultSlot) != null)
            {
                crafting.SwapWResult(itemDragHandler.ItemSlotUI.SlotIndex, SlotIndex);
            }
        }
        else if (itemDragHandler.ItemSlotUI.SlotType == "EquipSlot")
        {
            if ((itemDragHandler.ItemSlotUI as EquipSlot) != null)
            {
                inventory.SwapWEquip(itemDragHandler.ItemSlotUI.SlotIndex, SlotIndex);
            }
        }
        else
        {
            Debug.Log("Dragging from something that isn't an inventory slot or crafting slot? Error, not intended behavior");
        }

    }

    public override void SplitStack()
    {
        crafting.SplitStack(SlotIndex);
    }

    // move stack from crafting slot to inventory with shift click
    public override void QuickMoveStack()
    {
        if (inventory.inventoryShiftClick)
        {
            if (inventory.AddStack(ItemSlot))
            {
                // item successfully moved from crafting
                // empty crafting slot

                crafting.DeleteFromInventory(SlotIndex);
            }
        }
        //crafting.SwapWInventory(,SlotIndex);
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
        SlotType = "CraftingSlot";
    }
}