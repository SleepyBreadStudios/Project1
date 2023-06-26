/******************************************************************************
 * Enemy behavior file. Meant to be inherited by other enemy types.
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/
//#define Debug
using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


// consider making abstract
public class EnemyBehavior : NetworkBehaviour
{
    private string enemyName = null;

    [SerializeField]
    // type of drops the enemy drops when it dies(?)
    private GameObject item = null;

    private GameObject itemObj = null;

    [SerializeField]
    // health value
    private int health = 3;

    [SerializeField]
    private int maxHealth = 3;

    [SerializeField]
    // speed value
    private int speed;

    [SerializeField]
    // strength value
    private int strength;

    [SerializeField]
    // defense value
    private int defense;

    [SerializeField]
    // defense value
    private float aggroRange;

    private float time = 0.0f;

    [SerializeField]
    HealthBar healthBar;

    // getter method
    public string getName()
    {
        return enemyName;
    }

    public int getHealth()
    {
        return health;
    }

    // public ItemData getDrop()
    // {
    //     return item;
    // }

    public int getSpeed()
    {
        return speed;
    }

    public int getStrength()
    {
        return strength;
    }

    public int getDefense()
    {
        return defense;
    }

    public float getAggroRange()
    {
        return aggroRange;
    }

    public void Delete()
    {
        Destroy(this.gameObject);
    }


    // Function to allow item drops in enemy
    public void ItemDrop() {
        if (item.name == "Enemy3") {
            for (int i = 0; i < 10; i ++) {
                itemObj = Instantiate(item, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity) as GameObject;
                itemObj.GetComponent<NetworkObject>().Spawn(true);
            }
        } else {
            itemObj = Instantiate(item, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity) as GameObject;
            itemObj.GetComponent<NetworkObject>().Spawn(true);
        }
    }

    private void Awake()
    {
        healthBar = GetComponentInChildren<HealthBar>();
    }

    void Start()
    {
        healthBar.SetHealth(maxHealth);
        LoadServerRpc();
    }

    void Update()
    {
        //StartCoroutine(RegenerateTest());
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            //Debug.Log("hello");
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
        healthBar.UpdateHealth(health);
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

    // IEnumerator RegenerateTest() {
    //     if ((health > 0) && (health < maxHealth)) {
    //         yield return new WaitForSeconds(3);
    //         health = maxHealth;
    //         time = 0.0f;
    //         Debug.Log("regenerating");
    //     }
    // }

    public void UpdateTime() {
        if (health < 0) {
            time++;
        }
        Debug.Log(time);
    }
}
