using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
//using UnityEditor.Search;
using UnityEngine;

public class HeldDaggerBehavior : NetworkBehaviour
{
    // this class is purely to determine what the weapon does when triggered.
    // inheriting WeaponBehavior takes care of pretty much everything else.

    [SerializeField] private GameObject pivot;

    [SerializeField] private string weaponType;
    [SerializeField] private int strength;
    [SerializeField] private float knockback;
    [SerializeField] private float reload;

    private bool rotate = false;

    void Update()
    {
        if(rotate)
		{
            transform.RotateAround(pivot.transform.position, Vector3.back, 400 * Time.deltaTime);
            StartCoroutine(Lifetime());
        }
        
    }

    IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(0.2f);
        this.Delete();
    }

    public void Delete()
    {
        DespawnServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void DespawnServerRpc()
    {
        GetComponent<NetworkObject>().Despawn(true);
    }

    public override void OnNetworkDespawn()
    {
        Destroy(this.gameObject);
    }

    public override void OnNetworkSpawn()
    {
        rotate = true;
    }

    public string GetWeaponType()
    {
        return weaponType;
    }

    // getters and setters
    public int getStrength()
    {
        return strength;
    }

    public float getKnockback()
    {
        return knockback;
    }

    public float getReload()
    {
        return reload;
    }

}
