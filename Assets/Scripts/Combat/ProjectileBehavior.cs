/******************************************************************************
 * Shared template for items. Handles it's own deletion.
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/
//#define Debug
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


// consider making abstract
public class ProjectileBehavior : NetworkBehaviour
{
    // private Vector2 randLoc;

    [SerializeField]
    private float speed;

    [SerializeField]
    private int strength;

    [SerializeField]
    private string target;

    [SerializeField]
    // set to null or 0 for infinite projectiles
    private float lifetime;

    // [SerializeField]
    // private string target2;


    // To be tested for efficiency
    //private List<string>

    public float getSpeed()
    {
        return speed;
    }

    public int getStrength()
    {
        return strength;
    }

    protected virtual void Start()
    {
        if (lifetime > 0)
        {
            Destroy(gameObject, lifetime);
        }
    }

    protected virtual void Update()
    {
        transform.position += transform.up * Time.deltaTime * speed;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.CompareTag(target) || other.gameObject.CompareTag("Overworld"))
        {
            Destroy(gameObject);
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
