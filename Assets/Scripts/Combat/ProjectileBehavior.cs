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

    // [SerializeField]
    // private string target;

    // [SerializeField]
    // private string target2;


    // To be tested for efficiency
    //private List<string>

    public int getStrength()
    {
        return strength;
    }

    void Update()
    {
        transform.position += transform.up * Time.deltaTime * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Overworld"))
        {
            Destroy(this.gameObject);
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
