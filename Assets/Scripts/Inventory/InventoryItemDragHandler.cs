/******************************************************************************
 * Child class of ItemDragHandler. Extends for Inventory implementation.
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/

using UnityEngine;
using UnityEngine.EventSystems;

namespace DapperDino.Items.Inventories
{
    public class InventoryItemDragHandler : ItemDragHandler
    {

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                base.OnPointerUp(eventData);

                if (eventData.hovered.Count == 0)
                {
                    InventorySlot thisSlot = ItemSlotUI as InventorySlot;
                }
            }
        }
    }
}
