/******************************************************************************
 * Player manager, handles the connection of players to the host.
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/
//#define Debug
using DilmerGames.Core.Singletons;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayersManager : NetworkSingleton<PlayersManager>
{
    NetworkVariable<int> playersInGame = new NetworkVariable<int>();

    public int PlayersInGame
    {
        get
        {
            return playersInGame.Value;
        }
    }

    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            if(IsServer || IsHost)
                playersInGame.Value++;
                Debug.Log($"{id} just connected...");
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
        {
            if(IsServer)
                playersInGame.Value--;
                Debug.Log($"{id} just disconnected...");
        };
    }
}