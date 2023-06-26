using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableItem : ItemBehavior
{

    public override string GetItemEffect(Player2Behavior playerBehavior)
    {
        Debug.Log("Attempting to use a consumable item");
        // for testing health potion rn
        playerBehavior.HealPlayer();
        // tell inventory to consume item on use
        return "Consumable";
    }

    protected override void Start()
    {
        base.Start();
        leftOrRight = "right";
    }
}
