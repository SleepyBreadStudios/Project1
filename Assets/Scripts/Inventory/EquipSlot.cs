/******************************************************************************
 * Equip slot class. Inherits from ItemSlotUI. Handles equipment UI.
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

public class EquipSlot : ItemSlotUI, IDropHandler
{
    [SerializeField]
    private CraftingInventoryManager crafting = null;

    [SerializeField]
    private PlayerInventory inventory = null;

    [SerializeField]
    private EquipInventoryManager equipment = null;

    [SerializeField]
    private TextMeshProUGUI itemQuantityText = null;

    public override ItemSlot SlotItem
    {
        get { return ItemSlot; }
        set { }
    }

    public ItemSlot ItemSlot => equipment.GetSlotByIndex(SlotIndex);

    public override void OnDrop(PointerEventData eventData)
    {
        ItemDragHandler itemDragHandler = eventData.pointerDrag.GetComponent<ItemDragHandler>();

        // swap stack positions
        if (itemDragHandler.ItemSlotUI.SlotType == "InventorySlot")
        {
            if ((itemDragHandler.ItemSlotUI as InventorySlot) != null)
            {
                equipment.SwapWInventory(itemDragHandler.ItemSlotUI.SlotIndex, SlotIndex);
                equipment.CalculateCurrDef();
                equipment.CheckResistCold();
            }
        }
        // these should not be possible !
        //else if (itemDragHandler.ItemSlotUI.SlotType == "CraftingSlot")
        //{
        //    if ((itemDragHandler.ItemSlotUI as CraftingSlot) != null)
        //    {
        //        equipment.SwapWCrafting(itemDragHandler.ItemSlotUI.SlotIndex, SlotIndex);
        //    }
        //}
        //else if (itemDragHandler.ItemSlotUI.SlotType == "CraftingResultSlot")
        //{
        //    if ((itemDragHandler.ItemSlotUI as CraftingResultSlot) != null)
        //    {
        //        equipment.SwapWResult(itemDragHandler.ItemSlotUI.SlotIndex, SlotIndex);
        //    }
        //}
        else if (itemDragHandler.ItemSlotUI.SlotType == "EquipSlot")
        {
            if ((itemDragHandler.ItemSlotUI as EquipSlot) != null)
            {
                equipment.Swap(itemDragHandler.ItemSlotUI.SlotIndex, SlotIndex);
                equipment.CalculateCurrDef();
                equipment.CheckResistCold();
            }
        }
        else
        {
            Debug.Log("Dragging from something that isn't an inventory slot or crafting slot? Error, not intended behavior");
        }
    }

    // do nothing
    public override void SplitStack()
    {
        Debug.Log("Attempting to split stack equipment inventory");
    }

    // move stack from equipment slot to inventory with shift click
    public override void QuickMoveStack()
    {
        if (inventory.inventoryShiftClick)
        {
            if (inventory.AddStack(ItemSlot))
            {
                // empty crafting slot
                equipment.DeleteFromInventory(SlotIndex);
                // item successfully moved from crafting
                // update def 
                equipment.CalculateCurrDef();
                equipment.CheckResistCold();
                
                
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

        if (durability <= 0)
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
        equipment.DeleteFromInventory(slotIndex);
    }

    private void Awake()
    {
        SlotType = "EquipSlot";
        //ItemSlot = equipment.GetSlotByIndex(SlotIndex);
    }
}
