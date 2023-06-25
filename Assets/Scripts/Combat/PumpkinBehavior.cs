/******************************************************************************
 * Enemy behavior file. Meant to be inherited by other enemy types.
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/
//#define Debug
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;


// consider making abstract
public class PumpkinBehavior : EnemyBehavior
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    public bool isJumping;

    [SerializeField]
    // speed value
    private int speed;

    // Function to allow item drops in enemy

    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        animator = gameObject.GetComponent<Animator>();
        isJumping = false;

        // have to be SUPER CAREFUL there are no exceptions during runtime, otherwise this WILL NOT run
        InvokeRepeating("JumpTo", 0.0f, 2.0f);
    }

    void Update()
    {
        if (isJumping)
        {
            MoveToPlayer();
        }
    }

    // new move method, moves towards player
    public void MoveToPlayer()
    {
        if (GameObject.FindWithTag("Player") != null)
        {
            Vector2 playerLoc = GameObject.FindWithTag("Player").transform.position;
            if (playerLoc.x - transform.position.x < 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (playerLoc.x - transform.position.x > 0)
            {
                spriteRenderer.flipX = true;
            }
            transform.position = Vector2.MoveTowards(transform.position, playerLoc, Time.deltaTime * speed);
        }
    }

    // calls jump animation
    public void JumpTo()
    {
        if (GameObject.FindWithTag("Player") != null)
        {
            Vector2 playerLoc = GameObject.FindWithTag("Player").transform.position;
            if (Vector2.Distance(transform.position, playerLoc) < 5)
            {
                animator.SetTrigger("jump");
            } else
            {
                isJumping = false;
                animator.ResetTrigger("jump");
            }
        }
    }

    // called when animation starts
    public void StartJumping()
    {
        isJumping = true;
    }

    // called when animation finishes
    public void StopJumping()
    {
        isJumping = false;
    }
}
