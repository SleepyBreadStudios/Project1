/******************************************************************************
 * Shared scriptable template for overworld objects. Intended to make assets of this script
 * for each different overworld objects in game.
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class OverworldData : ScriptableObject
{
    // Name of the object
    [SerializeField]
    private string objectName = null;

    // Sprite used to represent object
    [SerializeField]
    public Sprite icon = null;

    // Health value for object. If 0, object is destroyed
    [SerializeField]
    public int health = 0;

    [SerializeField]
    public int maxHealth = 5;

    // Item that gets dropped when object is destroyed
    [SerializeField]
    private GameObject itemDrop = null;
    
    // getter method
    public string GetName()
    {
        return objectName;
    }

    public int GetHealth()
    {
        return health;
    }

    public GameObject GetItemDrop()
    {
        return itemDrop;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}

