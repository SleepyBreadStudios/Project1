/******************************************************************************
 * Shared template for items. Handles it's own deletion.
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/
//#define Debug
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// consider making abstract
public class ProjectileBehavior : MonoBehaviour
{
    private Vector2 randLoc;
    private float speed;

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
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<EnemyBehavior>().takeDamage();
            if(other.gameObject.GetComponent<EnemyBehavior>().getHealth() <= 0)
            {
                other.gameObject.GetComponent<EnemyBehavior>().Delete();
            }
            Destroy(this.gameObject);
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
