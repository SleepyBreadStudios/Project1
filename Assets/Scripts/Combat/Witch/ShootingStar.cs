using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ShootingStar : ProjectileBehavior
{
    private float variableSpeed;
    private Vector2 dest;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        variableSpeed = getSpeed();
        float randX = Random.Range(transform.position.x - 10f, transform.position.x + 10f);
        float randY = Random.Range(transform.position.y - 10f, transform.position.y + 10f);
        dest = new Vector2(randX, randY);
    }

    // Update is called once per frame
    protected override void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, dest, Time.deltaTime * variableSpeed);
        //variableSpeed *= 1.04f;
        if (transform.position.x == dest.x && transform.position.y == dest.y)
        {
            GetComponent<NetworkObject>().Despawn(true);
            Destroy(gameObject);
        }
    }
}
