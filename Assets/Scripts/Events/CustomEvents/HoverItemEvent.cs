using DapperDino.Items;
using UnityEngine;

namespace DapperDino.Events.CustomEvents
{
    [CreateAssetMenu(fileName = "New Hover Item Event", menuName = "Game Events/Hover Item Event")]
    public class HoverItemEvent : BaseGameEvent<ItemSlot> { }
}
