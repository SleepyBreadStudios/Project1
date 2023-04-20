using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : ItemSlotUI, IDropHandler
{
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
        if((itemDragHandler.ItemSlotUI as InventorySlot) != null)
        {
            inventory.Swap(itemDragHandler.ItemSlotUI.SlotIndex, SlotIndex);
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
}
