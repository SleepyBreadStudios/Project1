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

public class AppleBehavior : EnemyBehavior
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    public bool isShooting;

    // stuff needed for random move
    private Vector2 dest;

    [SerializeField]
    private float moveBias = 0.8f;

    [SerializeField]
    // seed
    private GameObject seed = null;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        isShooting = false;

        dest = transform.position;

        InvokeRepeating("FindPlayerServerRpc", 0.0f, 4.0f);
    }

    void Update()
    {
        if (!isShooting)
        {
            MoveServerRpc();
        }
    }

    [ServerRpc]
    public void MoveServerRpc()
    {
        float currX = transform.position.x;
        float currY = transform.position.y;
        if (currX == dest.x && currY == dest.y)
        {
            float randX = Random.Range(currX - moveBias, currX + moveBias);
            float randY = Random.Range(currY - moveBias, currY + moveBias);

            if (Random.Range(1, 1001) > 999)
            {
                if (randX - currX < 0)
                {
                    spriteRenderer.flipX = false;
                }
                else if (randX - currX > 0)
                {
                    spriteRenderer.flipX = true;
                }
                dest = new Vector2(randX, randY);
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, dest, Time.deltaTime * getSpeed());
    }

    [ServerRpc]
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
            isShooting = true;
            animator.SetTrigger("shoot");
        }
        else
        {
            isShooting = false;
            animator.ResetTrigger("shoot");
        }
    }

    public void Reload()
    {
        animator.ResetTrigger("shoot");
    }

    [ServerRpc]
    public void ShootServerRpc()
    {
        if (FindClosestPlayer() != null)
        {
            Vector2 playerLoc = FindClosestPlayer().transform.position;
            Vector2 appleLoc = transform.position;

            GameObject seedInstance = Instantiate(seed, appleLoc, Quaternion.identity);

            // IMPORTANT: get network to recognize object
            seedInstance.GetComponent<NetworkObject>().Spawn(true);

            seedInstance.transform.rotation = Quaternion.LookRotation(Vector3.forward, playerLoc - appleLoc);
        }
    }
}