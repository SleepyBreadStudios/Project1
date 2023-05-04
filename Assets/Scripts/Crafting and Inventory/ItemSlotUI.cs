/******************************************************************************
 * Abstract class for Inventory slots UI
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/
//#define Debug
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class ItemSlotUI : MonoBehaviour, IDropHandler
{
    [SerializeField]
    protected Image itemIconImage = null;

    public int SlotIndex { get; private set; }

    public string SlotType;

    public bool SlotLock;

    //public abstract HotbarItem SlotItem { get; set; }

    private void OnEnable()
    {
        UpdateSlotUI();
    }

    protected virtual void Start()
    {
        SlotIndex = transform.GetSiblingIndex();
        UpdateSlotUI();
    }

    public abstract void OnDrop(PointerEventData eventData);

    public abstract void UpdateSlotUI();

    protected virtual void EnableSlotUI(bool enable)
    {
        itemIconImage.enabled = enable;
    }
}
