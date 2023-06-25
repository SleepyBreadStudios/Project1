/******************************************************************************
 * Shared template for overworld objects. Handles it's own deletion.
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldBehavior : MonoBehaviour
{

    public OverworldData type;

    public OverworldData GetType() {
        return type;
    }

    // Allows object to regain it's health if it's not destroyed within a timeframe
    public void Regenerate() {
        if (type.health > 0 && type.health < type.maxHealth) {
            type.health = type.maxHealth;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Regenerate();
    }
}
