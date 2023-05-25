/******************************************************************************
 * Enemy behavior file. Meant to be inherited by other enemy types.
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/
//#define Debug
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


// consider making abstract
public class EnemyBehavior : NetworkBehaviour
{
    private Vector2 dest;

    // internal enemy name
    private string enemyName = null;

    [SerializeField]
    // type of drops the enemy drops when it dies(?)
    private GameObject item = null;

    private GameObject itemObj = null;

    // health value
    private int health = 3;

    // strength value
    private int strength;

    // defense value
    private int defense;

    [SerializeField]
    // speed value
    private int speed;

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

    public int getStrength()
    {
        return strength;
    }

    public int getDefense()
    {
        return defense;
    }

    public int getSpeed()
    {
        return speed;
    }

    public void move()
    {
        if (speed != 0) {
            if (transform.position.x == dest.x && transform.position.y == dest.y)
            {
                float randy = Random.Range
                    (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y);
                float randx = Random.Range
                    (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x);
                dest = new Vector2(randx, randy);
            }

            transform.position = Vector3.MoveTowards(transform.position, dest, Time.deltaTime * (float)0.3);
        }
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

    void Start()
    {
        dest = transform.position;
        LoadServerRpc();
    }

    void Update()
    {
        move();

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Projectile"))
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
