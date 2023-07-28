using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeItem : ItemBehavior
{
    [SerializeField]
    private List<string> RecipeText;

    public override string GetItemEffect(Player2Behavior playerBehavior)
    {
#if Debug
        Debug.Log("Attempting to use a recipe item");
#endif
        // for testing health potion rn
        playerBehavior.OpenRecipe(RecipeText);
        // tell inventory to consume item on use
        return "Recipe";
    }

    protected override void Awake()
    {
        base.Awake();
        leftOrRight = "right";
    }
}
