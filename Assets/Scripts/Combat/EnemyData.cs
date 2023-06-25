/******************************************************************************
 * Shared scriptable template for enemy. Intended to make assets of this script
 * for each different enemies in game.
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyData : ScriptableObject
{

    // Name of enemy
    [SerializeField]
    private string enemyName = null;

    // Sprite of enemies
    [SerializeField]
    private Sprite icon = null;

    // Item dropped from enemy death
    [SerializeField]
    private GameObject itemObj = null;

    // health value
    [SerializeField]
    private int health = 0;

    [SerializeField]
    private int maxHealth = 5;

    // Strength of enemy
    [SerializeField]
    private int strength;

    // defense value
    [SerializeField]
    private int defense;

    // Speed
    [SerializeField]
    private int speed;

    // getter method
    public string GetName()
    {
        return enemyName;
    }

    public int GetHealth()
    {
        return health;
    }

    public GameObject GetItemDrop()
    {
        return itemObj;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public int GetStrenth()
    {
        return strength;
    }

    public int GetDefense()
    {
        return defense;
    }

    public int GetSpeed()
    {
        return speed;
    }
}
