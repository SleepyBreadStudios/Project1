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
    private EquipInventoryManager equipment = null;

    [SerializeField]
    private PlayerInventory inventory = null;

    [SerializeField]
    private TextMeshProUGUI itemQuantityText = null;

	public override ItemSlot SlotItem
    {
		get { return ItemSlot; }
        set { }
	}

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
        inventory.SplitStack(SlotIndex);
    }

    // move stack from  inventory slot to crafting or equipment with shift click
    public override void QuickMoveStack()
    {
        if(inventory.inventoryShiftClick && inventory.craftingShiftClick)
        {
            if (crafting.AddStack(ItemSlot))
            {
                // item successfully moved to crafting
                // empty inventory slot
                inventory.DeleteFromInventory(SlotIndex);
            }
        }
        else if (inventory.inventoryShiftClick && !inventory.craftingShiftClick)
        {
            if (equipment.AddStack(ItemSlot, SlotIndex))
            {
                // item successfully moved to equipment
                // empty inventory slot
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

    //public override void OnDrop(PointerEventData eventData)
    //{


    //}

    private void Awake()
    {
        SlotType = "InventorySlot";
        //ItemSlot => inventory.GetSlotByIndex(SlotIndex);
}
}