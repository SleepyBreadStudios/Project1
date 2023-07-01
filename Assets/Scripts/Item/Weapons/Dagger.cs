using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.Search;
using UnityEngine;

public class Dagger : WeaponBehavior
{
    // this class is purely to determine what the weapon does when triggered.
    // inheriting WeaponBehavior takes care of pretty much everything else.

    [SerializeField] private GameObject pivot;

    void Update()
    {
        base.Update();
        if (isHeld())
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
}
