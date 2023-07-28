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
using UnityEngine.UI;

public class MushroomBehavior : EnemyBehavior
{
    [SerializeField]
    private GameObject item = null;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool isIdle;

    // stuff needed for random move
    private Vector2 dest;

    [SerializeField]
    private float moveBias = 0.8f;

    // how long it takes for mushroom to regenerate
    [SerializeField]
    private float sporeRespawnTime;

    private bool currRegenerating;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        isIdle = false;

        dest = transform.position;

        // have to be SUPER CAREFUL there are no exceptions during runtime, otherwise this WILL NOT run
        InvokeRepeating("FindPlayer", 0.0f, 2.0f);
    }

    void Update()
    {
        if (isIdle)
        {
            Move();
        }
        else if (!isIdle)
        {
            MoveAwayPlayer();
        }
    }

    public void Move()
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

    // new move method, moves towards player
    public void MoveAwayPlayer()
    {
        if (GameObject.FindWithTag("Player") != null)
        {
            Vector2 playerLoc = GameObject.FindWithTag("Player").transform.position;
            if (playerLoc.x - transform.position.x > 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (playerLoc.x - transform.position.x < 0)
            {
                spriteRenderer.flipX = true;
            }
            transform.position = Vector2.MoveTowards(transform.position, playerLoc, -1 * Time.deltaTime * getSpeed());
        }
    }

    // tests for nearest player in radius and locks on to them
    public void FindPlayer()
    {
        if (GameObject.FindWithTag("Player") != null)
        {
            GameObject[] playerLoc = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in playerLoc)
            {
                Vector2 loc = player.transform.position;
                if (Vector2.Distance(transform.position, loc) < getAggroRange())
                {
                    if (Vector2.Distance(transform.position, loc) < getAggroRange())
                    {
                        isIdle = false;
                    }
                    else
                    {
                        isIdle = true;
                    }
                }
            }
        }
    }

    public void HarvestSpores()
	{
        if(!currRegenerating)
		{
            GameObject itemObj = Instantiate(item, new Vector3(transform.position.x, transform.position.y, -1), Quaternion.identity) as GameObject;
            itemObj.GetComponent<NetworkObject>().Spawn(true);
            StartCoroutine("Regenerate");
            currRegenerating = true;
        }
	}

    public IEnumerator Regenerate()
    {
        yield return new WaitForSeconds(sporeRespawnTime);
        currRegenerating = false;
    }
}
