using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableItem : ItemBehavior
{
    [SerializeField]
    private float AmountToHeal;

    public override string GetItemEffect(Player2Behavior playerBehavior)
    {
#if Debug
        Debug.Log("Attempting to use a consumable item");
#endif
        // for testing health potion rn
        playerBehavior.HealPlayer(AmountToHeal);
        // tell inventory to consume item on use
        return "Consumable";
    }

    protected override void Awake()
    {
        base.Awake();
        leftOrRight = "right";
    }
}
