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

    private bool isBeingAttacked = false;

    [SerializeField]
    // health value
    private float health;

    [SerializeField]
    private float maxHealth;

    [SerializeField]
    private string ToolRequired = "";

    public GameObject GetItem()
    {
        return item;
    }

    private SpriteRenderer sprite;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void Delete()
    {
        Destroy(this.gameObject);
    }

        // Function to allow item drops in enemy
    public void ItemDrop() 
    {
        itemObj = Instantiate(item, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity) as GameObject;
        itemObj.GetComponent<NetworkObject>().Spawn(true);
    }

    // Overworld objects regenerate health after 3 seconds of not being hit
    public IEnumerator Regenerate() 
    {

        yield return new WaitForSeconds(5);
        isBeingAttacked = false;
        if (health < maxHealth) {
            health = maxHealth;
            sprite.color = new Color (1f, 1f, 1f, 1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isBeingAttacked) {
            StartCoroutine("Regenerate"); 
        } else {
            StopCoroutine("Regenerate");
        }    
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Tool"))
        {
            if(other.gameObject.GetComponent<WeaponBehavior>().GetWeaponType() == ToolRequired)
			{
                DamageServerRpc();
            }
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
        float alpha = health / maxHealth;
        sprite.color = new Color (1f, 1f, 1f, alpha);
        isBeingAttacked = true;

        if (health <= 0)
        {
            ItemDrop();
            GetComponent<NetworkObject>().Despawn(true);
        }
    }

    //public override void OnNetworkDespawn()
    //{
    //    Destroy(gameObject);
    //}
    
}
