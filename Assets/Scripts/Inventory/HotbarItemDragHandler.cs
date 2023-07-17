/******************************************************************************
 * Child class of ItemDragHandler. Adjusts hotbar so that no tool tip comes up.
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/

using UnityEngine;
using UnityEngine.EventSystems;

namespace DapperDino.Items.Inventories
{
    public class HotbarItemDragHandler : ItemDragHandler
    {
        public override void OnPointerEnter(PointerEventData eventData)
        {
            //Debug.Log("do nothing :)");
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            //Debug.Log("do nothing :)");
        }
    }
}
