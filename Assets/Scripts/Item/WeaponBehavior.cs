using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehavior : ItemBehavior
{
    private bool held;
    
    [SerializeField]
    private int strength;

    [SerializeField]
    private float knockback;

    [SerializeField]
    private float reload;

    public int getStrength()
    {
        return strength;
    }

    public float getKnockback()
    { 
        return knockback; 
    }

    public float getReload() 
    { 
        return reload; 
    }

    public bool isHeld()
    {
        return held;
    }

    protected override void Start()
    {
        base.Start();
        leftOrRight = "left";
        held = false;
    }
}
