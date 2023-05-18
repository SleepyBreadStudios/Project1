/******************************************************************************
* Shared template for enemies.
* 
* Authors: Alicia T, Jason N, Jino C
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviorSync : MonoBehaviour
{

    private EnemySpawner Spawner = null;
    private Vector2 dest;

    // internal enemy name
    private string enemyName = null;

    // type of drops the enemy drops when it dies(?)
    private ItemData item = null;

    // health value
    private int health; // Requested by Kelvin

    // strength value
    private int strength;

    // defense value
    private int defense;

    // speed value
    [SerializeField]
    private int speed;

    // getter method
    public string getName()
    {
        return enemyName;
    }

    public int getHealth()
    {
        return health;
    }

    public ItemData getDrop()
    {
        return item;
    }

    public int getStrength()
    {
        return strength;
    }

    public int getDefense()
    {
        return defense;
    }

    public int getSpeed()
    {
        return speed;
    }

    public void takeDamage()
    {
        health--;
    }

    public void move()
    {
        if (transform.position.x == dest.x && transform.position.y == dest.y)
        {
            float randy = Random.Range
                (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y);
            float randx = Random.Range
                (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x);
            dest = new Vector2(randx, randy);
        }

        transform.position = Vector3.MoveTowards(transform.position, dest, Time.deltaTime * (float)0.3);
    }
    public void Delete()
    {
        Destroy(this.gameObject);
    }

    void Start()
    {
        Spawner = GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>();
        health = Random.Range(2, 6);
        dest = transform.position;
    }

    void Update()
    {
        move();

    }

    void OnTriggerEnter2D(Collider2D other)
    {

    }
}
