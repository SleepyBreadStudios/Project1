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
    private Vector2 randLoc;

    [SerializeField]
    private float speed;

    [SerializeField]
    private string target;

    void Start()
    {
        speed = 10.0f;
    }

    void Update()
    {
        transform.position += transform.up * Time.deltaTime * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(target))
        {
            Destroy(this.gameObject);
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
