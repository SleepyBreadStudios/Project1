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

public class HotbarSlot : MonoBehaviour
{
    [SerializeField]
    private GameObject hotbarSelect = null;

    [SerializeField]
    private PlayerInventory inventory = null;

    public int SlotIndex { get; private set; }

    protected virtual void Start()
    {
        SlotIndex = transform.GetSiblingIndex() + 1;
        if(SlotIndex == 10)
        {
            SlotIndex = 0;
        }
        UpdateSlotUI();
    }

    public void UpdateSlotUI()
    {
        if(inventory.currHotbarNum != SlotIndex)
        {
            hotbarSelect.SetActive(false);
            return;
        }
        else
        {
            hotbarSelect.SetActive(true);
            //Debug.Log("doing smthn w slot " + inventory.currHotbarNum);
        }

    }

}