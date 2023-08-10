using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ActiveManager : MonoBehaviour
{
    private List<GameObject> tracking = new List<GameObject>();

    private float margin = 1f;

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject obj in tracking)
        {
            
        }

        //FindCamerasServerRpc();
    }

    public void Track(GameObject obj)
    {
        tracking.Add(obj);
    }

    [ServerRpc]
    public void FindCamerasServerRpc()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("MainCamera");
        Debug.Log("Cameras: " + players.Length);
    }

    [ServerRpc]
    public bool Calculate(Vector2 loc)
    {
        bool inBounds = false;

        GameObject[] cameras = GameObject.FindGameObjectsWithTag("MainCamera");
        Debug.Log("Cameras: " +  cameras.Length);
        for (int i = 0; i < cameras.Length; i++)
        {
            
        }

        return inBounds;
    }
}
