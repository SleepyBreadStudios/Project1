/******************************************************************************
 * Item Drag Handler - handles interaction with inventory [clicking, dragging, dropping].
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/

using DapperDino.Events.CustomEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class ItemDragHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
{
    [SerializeField]
    protected ItemSlotUI itemSlotUI = null;
    [SerializeField] protected HoverItemEvent onMouseStartHoverItem = null;
    [SerializeField] protected VoidEvent onMouseEndHoverItem = null;

    private CanvasGroup canvasGroup = null;
    private Transform originalParent = null;
    //private Vector3 originalScale;
    private bool isHovering = false;

    public ItemSlotUI ItemSlotUI => itemSlotUI;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnDisable()
    {
        if(isHovering)
        {
            // raise event
            onMouseEndHoverItem.Raise();
            isHovering = false;
        }
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // raise event
            originalParent = transform.parent;
            //originalScale = transform.localScale;

            // set parent to parent two up
            // please do not look at this hard code :) intent is so that the item is above all the other ui
            transform.SetParent(transform.parent.parent.parent.parent.parent.parent.parent);

            // ignore item that is being dragged and look at what is underneath it
            canvasGroup.blocksRaycasts = false;
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            //Debug.Log("Right click");
            itemSlotUI.SplitStack();
        }

        if (eventData.button == PointerEventData.InputButton.Left && Input.GetKey(KeyCode.LeftShift))
        {
            //Debug.Log("Shift click");
            itemSlotUI.QuickMoveStack();
        }
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        // if we are dragging an item, update the item to follow cursor
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            transform.position = Input.mousePosition;
        }
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            transform.SetParent(originalParent);
            transform.localPosition = Vector3.zero;
            //transform.localScale = originalScale;
            canvasGroup.blocksRaycasts = true;
        }
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        // raise event
        //Debug.Log("Hovering!");
        onMouseStartHoverItem.Raise(ItemSlotUI.SlotItem);
        isHovering = true;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        // raise event
        //Debug.Log("Stopped hovering");
        onMouseEndHoverItem.Raise();
        isHovering = false;
    }
}