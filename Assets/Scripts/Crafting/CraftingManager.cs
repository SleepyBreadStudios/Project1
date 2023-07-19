using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CraftingManager : MonoBehaviour
{
    [SerializeField]
    public List<Recipe> recipes;

    // Creates the item for the player and updates respective inventory and crafting slots
    // Once an item is crafted, should be sent in the crafted slot
    // Checks item present and compares across recipes
    public Tuple<Recipe, int> Craft(CraftingItem[] currCraftingList)//Dictionary<string, int> itemsInCrafting)
    {
        // check if it's a recipe
        var foundRecipe = recipes.Find(recipe => recipe.IsRecipeEqual(currCraftingList) == true);
        if (foundRecipe == null)
        {
            return null;
        }
        else
        {
            // return the item that matches the recipe found
            //var newItem = foundRecipe.GetCraftedItem();
            //return newItem;
            return Tuple.Create(foundRecipe, foundRecipe.GetCraftableCount(currCraftingList));
        }
    }
}