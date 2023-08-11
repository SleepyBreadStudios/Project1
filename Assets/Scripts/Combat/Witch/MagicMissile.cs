using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MagicMissile : ProjectileBehavior
{
    private GameObject targetPlayer;

    // Update is called once per frame
    protected override void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPlayer.transform.position, Time.deltaTime * getSpeed());
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        if (other.gameObject.CompareTag("Weapon") || other.gameObject.CompareTag("HeldWeapon"))
        {
            GetComponent<NetworkObject>().Despawn(true);
            Destroy(gameObject);
        }
    }

    public void SetTarget(GameObject player)
    {
        targetPlayer = player;
    }
}
