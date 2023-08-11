using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BodyBehavior : EnemyBehavior
{
    private int id;
    private GameObject head;
    private bool touching;

    [SerializeField]
    private GameObject icicle = null;
    
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("ShootServerRpc", 0.0f, 4.0f);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject == head ||
            collision.gameObject.GetComponent<BodyBehavior>() != null)
        {
            touching = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject == head ||
            collision.gameObject.GetComponent<BodyBehavior>() != null)
        {
            touching = false;
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (head != null)
        {
            head.GetComponent<HeadBehavior>().Remove(id);
        }
    }

    public void SetID(int num)
    {
        id = num;
    }

    public int GetID()
    {
        return id;
    }

    public void SetHead(GameObject obj)
    {
        head = obj;
    }

    public bool GetTouching()
    {
        return touching;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ShootServerRpc()
    {
        if (FindClosestPlayer() != null)
        {
            Vector2 playerLoc = FindClosestPlayer().transform.position;
            Vector2 appleLoc = transform.position;

            GameObject icicleInstance = Instantiate(icicle, appleLoc, Quaternion.identity);

            // IMPORTANT: get network to recognize object
            icicleInstance.GetComponent<NetworkObject>().Spawn(true);

            icicleInstance.transform.rotation = Quaternion.LookRotation(Vector3.forward, playerLoc - appleLoc);
        }
    }
}
