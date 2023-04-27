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
    //public Item item;
    //public int index;
    [SerializeField]
    private CraftingInventoryManager crafting = null;

    [SerializeField]
    private TextMeshProUGUI itemQuantityText = null;

    //override HotbarItem SlotItem
    //{
    //    get { return ItemSlot.item; }
    //}

    public ItemSlot ItemSlot => crafting.GetSlotByIndex(SlotIndex);

    public override void OnDrop(PointerEventData eventData)
    {
        ItemDragHandler itemDragHandler = eventData.pointerDrag.GetComponent<ItemDragHandler>();
        Debug.Log("Attempting to place item in crafting");
        // swap stack positions
        if (itemDragHandler.ItemSlotUI.SlotType == "InventorySlot")
        {
            Debug.Log("Found that it is from the inventory");
            if ((itemDragHandler.ItemSlotUI as InventorySlot) != null)
            {
                crafting.SwapCrafting(itemDragHandler.ItemSlotUI.SlotIndex, SlotIndex);
            }
        }
        else if(itemDragHandler.ItemSlotUI.SlotType == "CraftingSlot")
        {
            Debug.Log("Found that it is from the crafting");
            if ((itemDragHandler.ItemSlotUI as CraftingSlot) != null)
            {
                crafting.Swap(itemDragHandler.ItemSlotUI.SlotIndex, SlotIndex);
            }
        }
        else
        {
            Debug.Log("Dragging from something that isn't an inventory slot or crafting slot? Error, not intended behavior");
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
        SlotType = "CraftingSlot";
    }
}