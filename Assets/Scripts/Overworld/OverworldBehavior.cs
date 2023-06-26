/******************************************************************************
 * Shared template for overworld objects. Handles it's own deletion.
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/
using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OverworldBehavior : NetworkBehaviour
{
    [SerializeField]
    // type of drops the enemy drops when it dies(?)
    private GameObject item = null;

    private GameObject itemObj;

    [SerializeField]
    // health value
    private int health = 3;

    [SerializeField]
    private int maxHealth = 3;

    public GameObject GetItem()
    {
        return item;
    }

    public void Delete()
    {
        Destroy(this.gameObject);
    }

        // Function to allow item drops in enemy
    public void ItemDrop() {
        itemObj = Instantiate(item, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity) as GameObject;
        itemObj.GetComponent<NetworkObject>().Spawn(true);

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlayerProjectile") ||
            other.gameObject.CompareTag("Weapon") || other.gameObject.CompareTag("Projectile"))
        {
            DamageServerRpc();
        }
    }

    [ServerRpc]
    public void LoadServerRpc()
    {
        GetComponent<NetworkObject>().Spawn();
    }

    [ServerRpc]
    public void DamageServerRpc()
    {
        health--;
        if (health <= 0)
        {
            ItemDrop();
            GetComponent<NetworkObject>().Despawn(true);
        }
    }

    public override void OnNetworkDespawn()
    {
        Destroy(gameObject);
    }
    
}
