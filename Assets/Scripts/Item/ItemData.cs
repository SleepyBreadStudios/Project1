/******************************************************************************
 * Shared scriptable template for items. Intended to make assets of this script
 * for each different item in game.
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    [SerializeField]
    private string itemName = null;

    [SerializeField]
    private string displayName = null;

    [SerializeField]
    public Sprite icon = null;

    // for if an item drops as multiple
    [SerializeField]
    private int count = 1;

    // item should know how many it can stack up to
    [SerializeField]
    private int maxStack = 99;

    // for durability
    [SerializeField]
    private int startingCondition = 100;

    [SerializeField]
    private bool unique = false;

    // default for new items
    [SerializeField]
    public ItemBehavior itemBehavior = null;

    // consider making equips their own inherited scriptable
    // so other scriptables don't have these variables they don't use
    [SerializeField]
    private string equipType = null;

    [SerializeField]
    private float defense;

    [SerializeField]
    private string useText = "none";

    [SerializeField]
    private string itemDescription = "default item - no description yet";

    // getter method
    public string GetName()
    {
        return itemName;
    }

    public int GetCount()
    {
        return count;
    }

    public float GetDefense()
    {
        return defense;
    }

    public string GetEquipType()
    {
        return equipType;
    }

    public int GetMaxStackAmount()
    {
        return maxStack;
    }

    public bool GetUnique()
    {
        return unique;
    }

    public int GetStartingCondition()
    {
        return startingCondition;
    }

    public string GetUseText()
	{
        return useText;
	}

    public string GetItemDesc()
	{
        return itemDescription;
	}

    public string GetDisplayName()
	{
        return displayName;
	}
}
