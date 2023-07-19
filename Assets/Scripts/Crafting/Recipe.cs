/******************************************************************************
 * Recipe class used to store different recipes for the game. The crafting system should reference the
 * recipe class somehow and check the items in the crafting slot to see what item can be made. The number
 * of items and the type of items matter.
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class Recipe : ScriptableObject
{
    [SerializeField]
    private string recipeContents = "DefaultString";

    [System.Serializable]
    public class InspectorRecipe
	{
        public string item;
        public int count;
	}

    [SerializeField]
    public List<InspectorRecipe> inspectorRecipeList;

    public Dictionary<string, int> recipeList = new();

    [SerializeField]
    private ItemData craftedItem = null;

    [SerializeField]
    private int numOfItems = 0;

    // for the strings only of the recipe list
    public List<string> recipeKeys;

    public bool IsRecipeEqual(CraftingItem[] currCraftingList)//Dictionary<string, int> craftingInput)
    {
        //Debug.Log("Checking if recipe equal");
        Dictionary<string, int> craftingInput = new();
        for(int i = 0; i < currCraftingList.Length; i++)
		{
            if (currCraftingList[i] != null)
            {
                string item = currCraftingList[i].item;
                int count = currCraftingList[i].count;
                if (count != 0)
                {
                    // if the dict already has this item in it
                    if (craftingInput.ContainsKey(item))
                    {
                        // add them together
                        int oldCount = craftingInput[item];
                        craftingInput[item] = oldCount + count;
                    }
                    else
                    {
                        craftingInput.Add(item, count);
                        //Debug.Log(item + ": " + count);
                    }
                }
            }
          
		}

        var craftingStrings = craftingInput.Keys.ToList();
        // num of items in the recipe have to be equal to 
        // num of items in the crafting
        if (craftingStrings.Count != numOfItems)
        {
            return false;
        }
        for (int i = 0; i < craftingStrings.Count; i++)
        {
            // check if item is used in this recipe
            if(!(recipeContents.Contains(craftingStrings[i])))
            {
                // if one thing doesn't match it's already not equal
                return false;
            }
            else
			{
                // check if item count is enough for the recipe
                // need to have equal or more
                if (craftingInput[craftingStrings[i]] < recipeList[craftingStrings[i]])
                {
                    return false;
                }
			}
        }
        // also check if crafting input has all recipe items in case of
        // duplicate items accounting for recipe amount
        foreach(var key in recipeKeys)
		{
            if (!(craftingStrings.Contains(key)))
            {
                return false;
            }
		}
        // if all items were found in the recipe
        // the recipe is true
        return true;
    }

    public int GetCraftableCount(CraftingItem[] currCraftingList)
	{
        int lowestCount = 100;
        Dictionary<string, int> craftingInput = new();
        for (int i = 0; i < currCraftingList.Length; i++)
        {
            if (currCraftingList[i] != null)
            {
                string item = currCraftingList[i].item;
                int count = currCraftingList[i].count;
                if (count != 0)
                {
                    // if the dict already has this item in it
                    if (craftingInput.ContainsKey(item))
                    {
                        // add them together
                        int oldCount = craftingInput[item];
                        craftingInput[item] = oldCount + count;
                    }
                    else
                    {
                        craftingInput.Add(item, count);
                    }
                }
            }
        }
        foreach(var (key, value) in craftingInput)
		{
            int count = value / recipeList[key];
            if (count < lowestCount)
			{
                lowestCount = count;
            }
		}            
        return lowestCount;
	}

    public ItemData GetCraftedItem()
    {
        return craftedItem;
    }

    public int GetNumItems()
    {
        return numOfItems;
    }

    private void OnEnable()
	{
        // initialize dictionary
        for(int i = 0; i < inspectorRecipeList.Count; i++)
		{
            recipeList.Add(inspectorRecipeList[i].item, inspectorRecipeList[i].count);
		}
        // initialize string only list of recipe
        recipeKeys = recipeList.Keys.ToList();
    }
}