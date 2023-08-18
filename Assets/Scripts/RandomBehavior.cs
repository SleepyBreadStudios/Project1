using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private Vector2 dest;
    private float moveBias = 0.8f;
    private SpriteRenderer spriteRenderer;
    private float speed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveServerRpc();
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
        transform.position = Vector3.MoveTowards(transform.position, dest, Time.deltaTime * speed);
    }
}
