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
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.UI;

public class GhostBehavior : EnemyBehavior
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool idleLock;
    private bool resetting;

    // stuff needed for random move
    private Vector2 dest;

    [SerializeField]
    private float moveBias = 0.8f;

    [SerializeField]
    private float resetBias = 7.0f;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        idleLock = false;
        resetting = false;

        dest = transform.position;

        // have to be SUPER CAREFUL there are no exceptions during runtime, otherwise this WILL NOT run
        InvokeRepeating("FindPlayerServerRpc", 0.0f, 2.0f);
    }

    protected override void Update()
    {
        base.Update();
        if (resetting)
        {
            ResetServerRpc();
        }
        else
        {
            if (idleLock)
            {
                MoveToPlayerServerRpc();
            }
            else
            {
                MoveServerRpc();
            }
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
        transform.position = Vector3.MoveTowards(transform.position, dest, Time.deltaTime);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ResetServerRpc()
    {
        float currX = transform.position.x;
        float currY = transform.position.y;
        if (currX == dest.x && currY == dest.y)
        {
            animator.ResetTrigger("reset");
            Color c = GetComponent<Renderer>().material.color;
            GetComponent<Renderer>().material.color = new Color(c.r, c.g, c.b, 1.0f);
            resetting = false;
            GetComponent<Collider2D>().enabled = true;
        }
        transform.position = Vector3.MoveTowards(transform.position, dest, Time.deltaTime);
    }

    // new move method, moves towards player
    [ServerRpc(RequireOwnership = false)]
    public void MoveToPlayerServerRpc()
    {
        if (FindClosestPlayer() != null)
        {
            Vector2 playerLoc = FindClosestPlayer().transform.position;
            if (playerLoc.x - transform.position.x < 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (playerLoc.x - transform.position.x > 0)
            {
                spriteRenderer.flipX = true;
            }
            transform.position = Vector2.MoveTowards(transform.position, playerLoc, Time.deltaTime * getSpeed());
        }
    }

    // tests for nearest player in radius and locks on to them
    [ServerRpc(RequireOwnership = false)]
    public void FindPlayerServerRpc()
    {
        if (FindClosestPlayer() != null)
        {
            idleLock = true;
            animator.SetTrigger("sway");

        }
        else
        {
            idleLock = false;
            animator.ResetTrigger("sway");
            dest = transform.position;
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        if (other.gameObject.CompareTag("Player"))
        {
            animator.ResetTrigger("sway");
            animator.SetTrigger("reset");
            GetComponent<Collider2D>().enabled = false;
            resetting = true;
            Color c = GetComponent<Renderer>().material.color;
            GetComponent<Renderer>().material.color = new Color(c.r, c.g, c.b, c.a - 0.5f);
            dest = (Vector2)transform.position + Random.insideUnitCircle.normalized * (resetBias / 2);
            if (dest.x - transform.position.x < 0)
            {
                spriteRenderer.flipX = true;
            }
            else if (dest.x - transform.position.x > 0)
            {
                spriteRenderer.flipX = false;
            }
        }
    }
}
