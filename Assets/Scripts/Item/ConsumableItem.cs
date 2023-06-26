using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableItem : ItemBehavior
{

    public override bool GetItemEffect(Player2Behavior playerBehavior)
    {
        Debug.Log("Attempting to use a consumable item");
        // for testing health potion rn
        playerBehavior.HealPlayer();
        return true;
    }
}
