using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DurabilityItem : ItemBehavior
{
    public override string GetItemEffect(Player2Behavior playerBehavior)
    {
        Debug.Log("Attempting to use a durability item");

        DepleteDurability(5);
        // tell inventory what to do with item
        return "Durability";
    }

    protected override void Start()
    {
        base.Start();
        leftOrRight = "left";
    }
}
