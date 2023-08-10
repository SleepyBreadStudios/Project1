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
using UnityEngine.LowLevel;
using UnityEngine.UI;
//using static UnityEditor.Progress;


// consider making abstract
public class CauldronBehavior : NetworkBehaviour
{
    private string enemyName = null;
    public UnityEvent OnBegin, OnDone;
    private List<GameObject> spawned;
    private int previous;
    private Animator animator;
    private bool isSpawning;


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

    [SerializeField]
    private GameObject pumpkin;

    [SerializeField]
    private GameObject apple;

    [SerializeField]
    private GameObject carrot;

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
        spawned = new List<GameObject>();
        animator = gameObject.GetComponent<Animator>();
        isSpawning = false;
    }

    void Update()
    {
        //StartCoroutine(RegenerateTest());
        //Debug.Log("List length: " + spawned.Count);
        GameObject[] enemyLoc = GameObject.FindGameObjectsWithTag("Enemy");
        bool checkNear = false;
        foreach (GameObject enemy in enemyLoc)
        {
            Vector2 loc = enemy.transform.position;
            if (Vector2.Distance(transform.position, loc) < aggroRange)
            {
                checkNear = true;
            }
        }

        if (!checkNear)
        {
            SpawnEnemies();
        }
    }

    public void SpawnEnemies()
    {
        if (!isSpawning)
        {
            if (GameObject.FindWithTag("Player") != null)
            {
                GameObject[] playerLoc = GameObject.FindGameObjectsWithTag("Player");
                foreach (GameObject player in playerLoc)
                {
                    Vector2 loc = player.transform.position;
                    if (Vector2.Distance(transform.position, loc) < aggroRange)
                    {
                        isSpawning = true;
                        animator.SetTrigger("pew");
                    }
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void LoadServerRpc()
    {
        GetComponent<NetworkObject>().Spawn();
    }

    [ServerRpc]
    public void DamageServerRpc(int damage)
    {
        Debug.Log("Taking " + damage + " damage");
        for (int i = 0; i < damage; i ++)
        {
            health--;
        }
        healthBar.UpdateHealth(health);
        if (health <= 0)
        {
            //animator.SetTrigger("death");
            foreach (GameObject e in spawned)
            {
                e.GetComponent<NetworkObject>().Despawn(true);
                Destroy(e);
            }
            ItemDrop();
            GetComponent<NetworkObject>().Despawn(true);
            Debug.Log("Should be despawning");
            Destroy(gameObject);
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

    [ServerRpc]
    public void SpawnServerRpc()
    {
        GameObject e = null;
        Vector2 loc = transform.position;
        Vector2 randomLoc = new Vector2(loc.x + Random.Range(0.5f, 2.0f), loc.y + Random.Range(0.5f, 2.0f));
        int rand = Random.Range(1, 4);
        if (rand == 1)
        {
            e = Instantiate(pumpkin, randomLoc, Quaternion.identity);
        }
        else if (rand == 2)
        {
            e = Instantiate(apple, randomLoc, Quaternion.identity);
        }
        else if (rand == 3)
        {
            e = Instantiate(carrot, randomLoc, Quaternion.identity);
        }

        // IMPORTANT: get network to recognize object
        e.GetComponent<NetworkObject>().Spawn(true);
        spawned.Add(e);
    }

    private IEnumerator ResetKB()
    {
        yield return new WaitForSeconds(0.15f);
        rb.velocity = Vector2.zero;
        OnDone?.Invoke();
    }

    public void PewFinish()
    {
        isSpawning = false;
        animator.ResetTrigger("pew");
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
