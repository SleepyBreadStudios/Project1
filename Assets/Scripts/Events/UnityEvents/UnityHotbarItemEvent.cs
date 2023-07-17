using DapperDino.Items;
using System;
using UnityEngine.Events;

namespace DapperDino.Events.UnityEvents
{
    [Serializable] public class UnityHoverItemEvent : UnityEvent<ItemSlot> { }
}