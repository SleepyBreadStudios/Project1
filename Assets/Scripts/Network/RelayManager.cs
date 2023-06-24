using DilmerGames.Core.Singletons;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayManager : Singleton<RelayManager>
{
    public static RelayManager relay;
    [SerializeField]
    private string environment = "production";

    [SerializeField]
    private int maxNumberOfConnections = 4;

    public string joinCode = "n/a";

    public bool IsRelayEnabled => Transport != null && Transport.Protocol == Unity.Netcode.Transports.UTP.UnityTransport.ProtocolType.RelayUnityTransport;

    public Unity.Netcode.Transports.UTP.UnityTransport Transport => NetworkManager.Singleton.gameObject.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();

    public async Task<RelayHostData> SetupRelay()
    {
        Debug.Log($"Relay Server Starting With Max Connections: {maxNumberOfConnections}");

        InitializationOptions options = new InitializationOptions()
            .SetEnvironmentName(environment);

        await UnityServices.InitializeAsync(options);

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        Allocation allocation = await Relay.Instance.CreateAllocationAsync(maxNumberOfConnections);

        RelayHostData relayHostData = new RelayHostData
        {
            Key = allocation.Key,
            Port = (ushort) allocation.RelayServer.Port,
            AllocationID = allocation.AllocationId,
            AllocationIDBytes = allocation.AllocationIdBytes,
            IPv4Address = allocation.RelayServer.IpV4,
            ConnectionData = allocation.ConnectionData
        };

        relayHostData.JoinCode = await Relay.Instance.GetJoinCodeAsync(relayHostData.AllocationID);
        joinCode = relayHostData.JoinCode;

        Transport.SetRelayServerData(relayHostData.IPv4Address, relayHostData.Port, relayHostData.AllocationIDBytes,
                relayHostData.Key, relayHostData.ConnectionData);

        Debug.Log($"Relay Server Generated Join Code: {relayHostData.JoinCode}");

        return relayHostData;
    }

    public async Task<RelayJoinData> JoinRelay(string joinCode)
    {
        Debug.Log($"Client Joining Game With Join Code: {joinCode}");

        InitializationOptions options = new InitializationOptions()
            .SetEnvironmentName(environment);

        await UnityServices.InitializeAsync(options);

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        JoinAllocation allocation = await Relay.Instance.JoinAllocationAsync(joinCode);

        RelayJoinData relayJoinData = new RelayJoinData
        {
            Key = allocation.Key,
            Port = (ushort)allocation.RelayServer.Port,
            AllocationID = allocation.AllocationId,
            AllocationIDBytes = allocation.AllocationIdBytes,
            ConnectionData = allocation.ConnectionData,
            HostConnectionData = allocation.HostConnectionData,
            IPv4Address = allocation.RelayServer.IpV4,
            JoinCode = joinCode
        };

        Transport.SetRelayServerData(relayJoinData.IPv4Address, relayJoinData.Port, relayJoinData.AllocationIDBytes,
            relayJoinData.Key, relayJoinData.ConnectionData, relayJoinData.HostConnectionData);

        Debug.Log($"Client Joined Game With Join Code: {joinCode}");

        return relayJoinData;
    }

    private void Awake()
    {
        if (relay == null)
        {
            relay = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

}
