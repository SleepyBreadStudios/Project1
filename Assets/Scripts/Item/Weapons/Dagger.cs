using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.Search;
using UnityEngine;

public class Dagger : WeaponBehavior
{
    private bool isHeld = false;
    private Vector2 playerLoc;
    private Vector2 mousePos;

    public override string GetItemEffect(Player2Behavior playerBehavior)
    {
        DepleteDurability(5);
        // tell inventory what to do with item

        isHeld = true;
        playerLoc = playerBehavior.transform.position;
        mousePos = Input.mousePosition;

        Debug.Log("Item used at: " + playerLoc);
        Debug.Log("Item looking at: " + mousePos);

        return "Weapon";
    }

    protected override void Start()
    {
        base.Start();
        leftOrRight = "left";
    }

    void Update()
    {
        if (isHeld)
        {
            
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // if (other.gameObject.CompareTag("Player"))
        /*/ {
             Knockback(other.transform.position);
            if (other.gameObject.CompareTag("PlayerProjectile") ||
                other.gameObject.CompareTag("Weapon") || other.gameObject.CompareTag("Projectile"))
            {
                DamageServerRpc();
                //Debug.Log("Hitting");
            }
        // }*/
    }
}
