/******************************************************************************
 * Apple behavior file. Inherits Enemy Behavior
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/
//#define Debug
using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.UI;

public class WitchBehavior : EnemyBehavior
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private float moveBias = 0.8f;

    [SerializeField]
    // magic missile attack
    private GameObject magic = null;

    [SerializeField]
    // shooting star attack
    private GameObject star = null;

    [SerializeField]
    // fire wall attack
    private GameObject fire = null;

    [SerializeField]
    private GameObject cauldron = null;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        InvokeRepeating("FindPlayerServerRpc", 0.0f, 6.0f);
    }

    protected override void Update()
    {
        if (cauldron == null)
        {
            GetComponent<NetworkObject>().Despawn(true);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void FindPlayerServerRpc()
    {
        if (FindClosestPlayer() != null)
        {
            Vector2 loc = FindClosestPlayer().transform.position;
            if (loc.x - transform.position.x < 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (loc.x - transform.position.x > 0)
            {
                spriteRenderer.flipX = true;
            }
            ShootServerRpc();
            //animator.SetTrigger("shoot");
        }
    }

    public void Reload()
    {
        //animator.ResetTrigger("shoot");
    }

    [ServerRpc(RequireOwnership = false)]
    public void ShootServerRpc()
    {
        int rand = Random.Range(1, 4);
        if (rand == 1)
        {
            MagicServerRpc();
        }
        else if (rand == 2)
        {
            StarServerRpc();
        }
        else if (rand == 3)
        {
            FireServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void MagicServerRpc()
    {
        if (GameObject.FindWithTag("Player") != null)
        {
            GameObject[] playerLoc = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in playerLoc)
            {
                Vector2 loc = player.transform.position;
                float distCalc = Vector2.Distance(transform.position, loc);
                if (distCalc < getAggroRange())
                {
                    GameObject magicInstance = Instantiate(magic, transform.position, Quaternion.identity);
                    magicInstance.GetComponent<NetworkObject>().Spawn(true);
                    magicInstance.GetComponent<MagicMissile>().SetTarget(player);
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void StarServerRpc()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject starInstance = Instantiate(star, transform.position, Quaternion.identity);
            starInstance.GetComponent<NetworkObject>().Spawn(true);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void FireServerRpc()
    {
        for (int i = 0; i < 7; i++)
        {
            float randX = Random.Range(transform.position.x - getAggroRange(), transform.position.x + getAggroRange());
            float randY = Random.Range(transform.position.y - getAggroRange(), transform.position.y + getAggroRange());
            GameObject fireInstance = Instantiate(fire, new Vector2(randX, randY), Quaternion.identity);
            fireInstance.GetComponent<NetworkObject>().Spawn(true);
        }
    }
}