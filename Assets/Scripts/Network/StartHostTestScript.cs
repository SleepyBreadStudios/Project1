
using DilmerGames.Core.Singletons;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StartHostTestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SetupRelay();
    }


    public async void SetupRelay()
    {
        // if (RelayManager.Instance.IsRelayEnabled)
        //     await RelayManager.Instance.SetupRelay();

        // if (NetworkManager.Singleton.StartHost())
        // {
        //     Debug.Log("Host started...");
        // }
        // else
        //     Debug.Log("Unable to start host...");
    }
}
