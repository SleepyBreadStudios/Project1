using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostConsumable : ItemBehavior
{
    [SerializeField]
    private float PotionDuration;

    public override string GetItemEffect(Player2Behavior playerBehavior)
    {
#if Debug
        Debug.Log("Attempting to use a consumable item");
#endif
        Debug.Log("Attempting to use a consumable item");
        // for testing health potion rn
        playerBehavior.ResistColdDuration(PotionDuration);
        // tell inventory to consume item on use
        return "Consumable";
    }

    protected override void Awake()
    {
        base.Awake();
        leftOrRight = "right";
    }


}
