using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class SpikeLeadBehavior : ProjectileBehavior
{
    [SerializeField]
    private GameObject spike;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        InvokeRepeating("DropSpike", 0.0f, 0.1f);
    }

    // Update is called once per frame
    void DropSpike()
    {
        GameObject spikeInstance = Instantiate(spike, this.transform.position, Quaternion.identity);

        // IMPORTANT: get network to recognize object
        spikeInstance.GetComponent<NetworkObject>().Spawn(true);
    }
}
