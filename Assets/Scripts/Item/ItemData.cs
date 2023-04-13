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
    private Sprite icon = null;

    // for if an item drops as multiple
    [SerializeField]
    private int count = 1;

    // for durability
    [SerializeField]
    private int startingCondition = 100;
    
    // getter method
    public string GetName()
    {
        return itemName;
    }

    public int GetCount()
    {
        return count;
    }
}
