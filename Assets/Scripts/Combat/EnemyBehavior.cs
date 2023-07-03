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
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
//using static UnityEditor.Progress;


// consider making abstract
public class EnemyBehavior : NetworkBehaviour
{
    private string enemyName = null;
    public UnityEvent OnBegin, OnDone;

    [SerializeField]
    private Rigidbody2D rb;

    // type of drops the enemy drops when it dies(?)
    [SerializeField] private List<GameObject> dropTable = new List<GameObject>();
    // chances correspond to above list, float entered is the percentage chance
    // i.e. 59.1f is 59.1%
    [SerializeField] private List<float> dropChance = new List<float>();

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
        for (int i = 0; i < dropTable.Count; i++)
        {
            if (dropChance[i] >= Random.Range(0, 100))
            {
                GameObject drop = Instantiate(dropTable[i], new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
                drop.GetComponent<NetworkObject>().Spawn(true);
            }
        }
    }

    public void attacked(int damage, float knockback, Vector2 player)
    {
        DamageServerRpc(damage);
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
        if (other.gameObject.CompareTag("Weapon"))
        {
            WeaponBehavior weaponBehavior = other.gameObject.GetComponent<WeaponBehavior>();
            if (weaponBehavior != null)
            {
                DamageServerRpc(weaponBehavior.getStrength());
                KnockbackServerRpc(other.transform.position, weaponBehavior.getKnockback());
            }
        }
        else if (other.gameObject.CompareTag("PlayerProjectile"))
        {
            if (other.gameObject.GetComponent<ProjectileBehavior>() != null)
            {
                DamageServerRpc(other.gameObject.GetComponent<ProjectileBehavior>().getStrength());
            }
        }
    }

    [ServerRpc]
    public void LoadServerRpc()
    {
        GetComponent<NetworkObject>().Spawn();
    }

    [ServerRpc]
    public void DamageServerRpc(int damage)
    {
        for (int i = 0; i < damage; i ++)
        {
            health--;
        }
        healthBar.UpdateHealth(health);
        if (health <= 0)
        {
            ItemDrop();
            GetComponent<NetworkObject>().Despawn(true);
        }
    }

    [ServerRpc]
    public void KnockbackServerRpc(Vector2 applier, float force)
    {
        StopAllCoroutines();
        OnBegin?.Invoke();
        Vector2 direction = ((Vector2)transform.position - applier).normalized;
        rb.AddForce(direction * force, ForceMode2D.Impulse);
        StartCoroutine(ResetKB());
    }

    private IEnumerator ResetKB()
    {
        yield return new WaitForSeconds(0.15f);
        rb.velocity = Vector2.zero;
        OnDone?.Invoke();
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

    // public void UpdateTime() {
    //     if (health < 0) {
    //         time++;
    //     }
    //     Debug.Log(time);
    // }
}
