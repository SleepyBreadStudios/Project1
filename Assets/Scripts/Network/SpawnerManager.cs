using DilmerGames.Core.Singletons;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class SpawnerManager : NetworkBehaviour
{
    private readonly string[] itemPrefabs = { "triangle", "capsule", "checkmark", "circle", "diamond", "flower", "hexagon", "hexagonpoint", "roundedsquare", "star" };
    [SerializeField]
    private int numToSpawn = 0;

    private List<GameObject> spawnedItems = new();

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += () =>
        {
            SpawnStart();
        };

    }

    public void SpawnStart()
    {
        for (int i = 0; i < numToSpawn; i++)
        {
            string toLoad = "Items/" + itemPrefabs[Random.Range(0, 10)];
            float randy = Random.Range
                (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y);
            float randx = Random.Range
                (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x);
            Vector3 randLoc = new Vector3(randx, randy, 0);
            GameObject newItem = Instantiate(Resources.Load(toLoad), randLoc, Quaternion.identity) as GameObject;
            newItem.GetComponent<NetworkObject>().Spawn();
            //spawnedItems.Add(newItem);
            //AddtoClientRpc(newItem);
        }
    }

    public bool SpawnObjects(Vector3 pos, ulong networkId, GameObject gameObj)
    {
        //Debug.Log("Attempting to spawn object");
        if (!IsServer)
        {
            return false;
        }

        string toLoad = "Items/" + itemPrefabs[Random.Range(0, 10)];
        GameObject newItem = Instantiate(Resources.Load(toLoad), pos, Quaternion.identity) as GameObject;
        //GameObject newItem = Instantiate(objectPrefab, pos, Quaternion.identity);
        newItem.GetComponent<NetworkObject>().Spawn();
        spawnedItems.Add(newItem);
        //AddtoClientRpc(newItem);
        //NetworkObject m_SpawnedNetworkObject = UnityEngine.GameObject.FindObjectsOfType<NetworkObject>().Where(n => n.NetworkObjectId == networkId).FirstOrDefault(); ;
        //if (m_SpawnedNetworkObject != null)
        //{
        //    gameObj.GetComponent<NetworkObject>().Despawn(true);
        //}

        if (gameObj.GetComponent<NetworkObject>().IsSpawned)
        {
            gameObj.GetComponent<NetworkObject>().Despawn();
            //spawnedItems.Remove(gameObj);
            //RemoveFromClientClientRpc(gameObj);
        }
        return true;
    }

    public void AddObject(GameObject newItem)
    {
        spawnedItems.Add(newItem);
    }

    public void RemoveObject(GameObject objToRemove)
    {
        spawnedItems.Remove(objToRemove);
    }

    //[ClientRpc]
    //public void AddtoClientRpc(GameObject newItem)
    //{
    //    spawnedItems.Add(newItem);
    //}

    //[ClientRpc]
    //public void RemoveFromClientClientRpc(GameObject gameObj)
    //{
    //    spawnedItems.Remove(gameObj);
    //}

    //protected override void OnSynchronize<T>(ref BufferSerializer<T> serializer)
    //{

    //    base.OnSynchronize(ref serializer);
    //}
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        //if(spawnedItems.Count == 0)
        //{
        //    Debug.Log("Hello");
        //}
        //else
        //{
        //    Debug.Log("???");
        //}
    }

    [ServerRpc]
    public void PrintObjectsServerRpc()
    {
        for (int i = 0; i < spawnedItems.Count; i++)
        {
            //Debug.Log(spawnedItems[i].transform.position);
            if(spawnedItems[i] != null)
            {
                Debug.Log(spawnedItems[i].name + ": " + spawnedItems[i].transform.position);
            }
        }
    }

    [ClientRpc]
    public void PrintObjectsClientRpc()
    {
        if (IsClient && !IsHost)
        {
            for (int i = 0; i < spawnedItems.Count; i++)
            {
                if(spawnedItems[i] != null)
                {
                    Debug.Log(spawnedItems[i].name + ": " + spawnedItems[i].transform.position);
                }
            }
        }

    }


}