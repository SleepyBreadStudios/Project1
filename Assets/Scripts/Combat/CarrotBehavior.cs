/******************************************************************************
 * Pumpkin behavior file. Inherits Enemy Behavior
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/
//#define Debug
using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.UI;

public class CarrotBehavior : EnemyBehavior
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool isSpinning;
    private bool idleLock;

    // stuff needed for random move
    private Vector2 dest;

    // stuff needed for drill move
    private Vector2 target;

    private GameObject indInstance;

    [SerializeField]
    private float moveBias = 0.8f;

    // indicator UI
    [SerializeField]
    private GameObject indicator = null;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        isSpinning = false;
        idleLock = false;

        dest = transform.position;

        // have to be SUPER CAREFUL there are no exceptions during runtime, otherwise this WILL NOT run
        InvokeRepeating("FindPlayerServerRpc", 0.0f, 2.0f);
    }

    void Update()
    {
        Color c = GetComponent<Renderer>().material.color;
        if (isSpinning)
        {
            c = new Color(c.r, c.g, c.b, c.a - (1.0f * Time.deltaTime));
            GetComponent<Renderer>().material.color = c;
        }
        if (c.a <= 0)
        {
            GetComponent<Collider2D>().enabled = false;
            //IndicateServerRpc(target);
            transform.position = Vector2.MoveTowards(transform.position, target, Time.deltaTime * getSpeed());
            if (transform.position.x == target.x && transform.position.y == target.y)
            {
                //DeindicateServerRpc();
                c = new Color(c.r, c.g, c.b, 1.0f);
                GetComponent<Renderer>().material.color = c;
                GetComponent<Collider2D>().enabled = true;
                idleLock = false;
                isSpinning = false;
                animator.ResetTrigger("spin");
            }
        } 
        if (!idleLock) 
        {
            //Move();
        }
    }

    [ServerRpc(RequireOwnership = false)]
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

    // tests for nearest player in radius and locks on to them
    [ServerRpc(RequireOwnership = false)]
    public void FindPlayerServerRpc()
    {
        if (!isSpinning)
        {
            if (FindClosestPlayer() != null)
            {
                target = FindClosestPlayer().transform.position;
                if (target.x - transform.position.x < 0)
                {
                    spriteRenderer.flipX = false;
                }
                else if (target.x - transform.position.x > 0)
                {
                    spriteRenderer.flipX = true;
                }

                idleLock = true;
                isSpinning = true;
                animator.SetTrigger("spin");
            }
        }
    }

    [ServerRpc]
    public void IndicateServerRpc(Vector2 loc)
    {
        if (indInstance == null)
        {
            indInstance = Instantiate(indicator, loc, Quaternion.identity);
            indInstance.GetComponent<NetworkObject>().Spawn(true);
        }
    }

    [ServerRpc]
    public void DeindicateServerRpc()
    {
        indInstance.GetComponent<NetworkObject>().Despawn(true);
        Destroy(indInstance);
        indInstance = null;
    }
}
