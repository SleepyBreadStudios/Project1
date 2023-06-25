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
using UnityEngine.LowLevel;
using UnityEngine.UI;


// consider making abstract
public class AppleBehavior : EnemyBehavior
{
    private Vector2 dest;
    public bool isShooting;
    private float moveBias = 0.8f;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    // speed value
    private int speed;

    [SerializeField]
    // seed
    private GameObject seed = null;

    void Start()
    {
        dest = transform.position;
        isShooting = false;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        animator = gameObject.GetComponent<Animator>();
        InvokeRepeating("FindPlayer", 0.0f, 4.0f);
    }

    void Update()
    {
        Move();
    }

    public void Move()
    {
        if(!isShooting)
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
            transform.position = Vector3.MoveTowards(transform.position, dest, Time.deltaTime * speed);
        }
    }

    public void FindPlayer()
    {
        if (GameObject.FindWithTag("Player") != null)
        {
            Vector2 playerLoc = GameObject.FindWithTag("Player").transform.position;
            if (Vector2.Distance(transform.position, playerLoc) < 8)
            {
                if (playerLoc.x - transform.position.x < 0)
                {
                    spriteRenderer.flipX = false;
                }
                else if (playerLoc.x - transform.position.x > 0)
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
    }

    public void Reload()
    {
        animator.ResetTrigger("shoot");
    }

    [ServerRpc]
    public void ShootServerRpc()
    {
        if (GameObject.FindWithTag("Player") != null)
        {
            Vector2 playerLoc = GameObject.FindWithTag("Player").transform.position;
            Vector2 appleLoc = this.transform.position;
            GameObject seedInstance = Instantiate(seed, appleLoc, Quaternion.identity) as GameObject;

            // IMPORTANT: get network to recognize object
            seedInstance.GetComponent<NetworkObject>().Spawn(true);

            seedInstance.transform.rotation = Quaternion.LookRotation(Vector3.forward, playerLoc - appleLoc);
        }
    }
}