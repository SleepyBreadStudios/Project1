/******************************************************************************
 * Inventory slot class. Inherits from ItemSlotUI. Handles inventory UI.
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

public class InventorySlot : ItemSlotUI, IDropHandler
{
    [SerializeField]
    private CraftingInventoryManager crafting = null;

    [SerializeField]
    private PlayerInventory inventory = null;

    [SerializeField]
    private TextMeshProUGUI itemQuantityText = null;

    //override HotbarItem SlotItem
    //{
    //    get { return ItemSlot.item; }
    //}

    public ItemSlot ItemSlot => inventory.GetSlotByIndex(SlotIndex);

    public override void OnDrop(PointerEventData eventData)
    {
        ItemDragHandler itemDragHandler = eventData.pointerDrag.GetComponent<ItemDragHandler>();

        // swap stack positions
        if (itemDragHandler.ItemSlotUI.SlotType == "InventorySlot")
        {
            if ((itemDragHandler.ItemSlotUI as InventorySlot) != null)
            {
                inventory.Swap(itemDragHandler.ItemSlotUI.SlotIndex, SlotIndex);
            }
        }
        else if (itemDragHandler.ItemSlotUI.SlotType == "CraftingSlot")
        {
            if ((itemDragHandler.ItemSlotUI as CraftingSlot) != null)
            {
                inventory.SwapWCrafting(itemDragHandler.ItemSlotUI.SlotIndex, SlotIndex);
            }
        }
        else if (itemDragHandler.ItemSlotUI.SlotType == "CraftingResultSlot")
        {
            if ((itemDragHandler.ItemSlotUI as CraftingResultSlot) != null)
            {
                inventory.SwapWResult(itemDragHandler.ItemSlotUI.SlotIndex, SlotIndex);
            }
        }
        else
        {
            Debug.Log("Dragging from something that isn't an inventory slot or crafting slot? Error, not intended behavior");
        }
    }

    public override void SplitStack()
    {
        inventory.SplitStack(SlotIndex);
    }

    // move stack from  inventory slot to crafting with shift click
    public override void QuickMoveStack()
    {
        if(inventory.inventoryShiftClick)
        {
            if (crafting.AddStack(ItemSlot))
            {
                // item successfully moved from crafting
                // empty crafting slot
                inventory.DeleteFromInventory(SlotIndex);
            }
        }

    }

    public override void UpdateSlotUI()
    {
        if (ItemSlot.item == null || ItemSlot.IsEmptySlot())
        {
            EnableSlotUI(false);
            return;
        }
        // refactoring needed for durability changing transparency
        var color = itemIconImage.color;
        float durability = 0.01f * ItemSlot.GetDurability();
        color.a = durability;
        itemIconImage.color = color;

        if(durability <= 0)
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

    public void DragDelete(int slotIndex)
    {
        inventory.DeleteFromInventory(slotIndex);
    }

    private void Awake()
    {
        SlotType = "InventorySlot";
    }
}