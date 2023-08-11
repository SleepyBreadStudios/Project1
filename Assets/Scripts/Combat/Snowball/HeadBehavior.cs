using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class HeadBehavior : EnemyBehavior
{
    private List<GameObject> bodies = new List<GameObject>();
    private int index;
    private Vector2 dest;

    [SerializeField]
    private GameObject body = null;

    [SerializeField]
    private float followBias = 0.1f;
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 8; i++)
        {
            GameObject segment = Instantiate(body, transform.position, Quaternion.identity);
            segment.GetComponent<NetworkObject>().Spawn(true);

            if (segment.GetComponent<BodyBehavior>() != null)
            {
                segment.GetComponent<BodyBehavior>().SetHead(gameObject);
                segment.GetComponent<BodyBehavior>().SetID(i);
            }

            bodies.Add(segment);
            Debug.Log("Spawned segment " + i);
        }

        dest = transform.position;

        index = 0;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (bodies.Count == 0)
        {
            ItemDrop();
            GetComponent<NetworkObject>().Despawn(true);
            Destroy(gameObject);
        }

        MoveServerRpc();

        foreach (GameObject segment in bodies)
        {
            if (!segment.GetComponent<BodyBehavior>().GetTouching())
            {
                segment.gameObject.transform.position = 
                    Vector3.MoveTowards(segment.gameObject.transform.position, transform.position, Time.deltaTime * getSpeed());
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
            dest = new Vector2(Random.Range(-60f, 60f), Random.Range(-120f, 60f));
        }
        transform.position = Vector3.MoveTowards(transform.position, dest, Time.deltaTime * getSpeed());
    }

    public void Remove(int id)
    {
        foreach (GameObject segment in bodies)
        {
            if (segment.GetComponent<BodyBehavior>() != null)
            {
                if (segment.GetComponent<BodyBehavior>().GetID() == id)
                {
                    bodies.Remove(segment);
                    break;
                }
            }
        }
    }
}
