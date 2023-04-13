using System;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Http;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Relay;
using NetworkEvent = Unity.Networking.Transport.NetworkEvent;

    public class Manager : NetworkBehaviour
    {
        const int m_MaxConnections = 4;

		//public string RelayJoinCode;

        /// <summary>
        /// The textbox displaying the Player Id.
        /// </summary>
        public Text PlayerIdText;

        // /// <summary>
        // /// The dropdown displaying the region.
        // /// </summary>
        public Dropdown RegionsDropdown;

        /// <summary>
        /// The textbox displaying the Allocation Id.
        /// </summary>
        public Text HostAllocationIdText;

        /// <summary>
        /// The textbox displaying the Join Code.
        /// </summary>
        public Text JoinCodeText;

        /// <summary>
        /// The Input Field for user to input join code.
        /// </summary>
        public InputField joinCodeInput;

        /// <summary>
        /// The textbox displaying the Allocation Id of the joined allocation.
        /// </summary>
        public Text PlayerAllocationIdText;

        Guid hostAllocationId;
        Guid playerAllocationId;
        string allocationRegion = "";
        string joinCode = "n/a";
        //string joinCodeInputText = "n/a";
        string playerId = "Not signed in";
        string autoSelectRegionName = "auto-select (QoS)";
        int regionAutoSelectIndex = 0;
        List<Region> regions = new List<Region>();
        List<string> regionOptions = new List<string>();

        public async void OnAuthenticate()
        {
            try
            {
                await UnityServices.InitializeAsync();
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                playerId = AuthenticationService.Instance.PlayerId;
                Debug.Log($"Signed in. Player ID: {playerId}");
                UpdateUI();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        /// <summary>
        /// Event handler for when the Get Regions button is clicked.
        /// </summary>
        public async void OnRegion()
        {
            Debug.Log("Host - Getting regions.");
            var allRegions = await RelayService.Instance.ListRegionsAsync();
            regions.Clear();
            regionOptions.Clear();
            foreach (var region in allRegions)
            {
                Debug.Log(region.Id + ": " + region.Description);
                regionOptions.Add(region.Id);
                regions.Add(region);
            }
            UpdateUI();
        }

        /// <summary>
        /// Event handler for when the Allocate button is clicked.
        /// </summary>
        public async void OnAllocate()
        {
            Debug.Log("Host - Creating an allocation.");

            // Determine region to use (user-selected or auto-select/QoS)
            string region = GetRegionOrQosDefault();
            Debug.Log($"The chosen region is: {region ?? autoSelectRegionName}");
            await AllocateRelayServerAndGetJoinCode(m_MaxConnections, region);

            ConfigureTransportAndStartNgoAsHost();

            UpdateUI();
        }

        string GetRegionOrQosDefault()
        {
            // Return null (indicating to auto-select the region/QoS) if regions list is empty OR auto-select/QoS is chosen
            if (!regions.Any() || RegionsDropdown.value == regionAutoSelectIndex)
            {
                return null;
            }
            // else use chosen region (offset -1 in dropdown due to first option being auto-select/QoS)
            return regions[RegionsDropdown.value - 1].Id;
        }

        public async Task<RelayServerData> AllocateRelayServerAndGetJoinCode(int maxConnections, string region = null)
        {
            // Important: Once the allocation is created, you have ten seconds to BIND
            Allocation allocation;
            try
            {
                allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections, region);                            
                hostAllocationId = allocation.AllocationId;
                allocationRegion = allocation.Region;
                Debug.Log($"Host Allocation ID: {hostAllocationId}, region: {allocationRegion}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Relay create allocation request failed {e.Message}");
                throw;
            }

            Debug.Log($"server: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
            Debug.Log($"server: {allocation.AllocationId}");

            try
            {
                joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                Debug.Log("Host - Got Join Code: " + joinCode);
            }
            catch
            {
                Debug.LogError("Relay create join code request failed");
                throw;
            }

            return new RelayServerData(allocation, "dtls");
        }

        IEnumerator ConfigureTransportAndStartNgoAsHost()
        {
            Debug.Log("Configuring transport and starting host");
            var serverRelayUtilityTask = AllocateRelayServerAndGetJoinCode(m_MaxConnections);
            while (!serverRelayUtilityTask.IsCompleted)
            {
                yield return null;
            }
            if (serverRelayUtilityTask.IsFaulted)
            {
                Debug.LogError("Exception thrown when attempting to start Relay Server. Server not started. Exception: " + serverRelayUtilityTask.Exception.Message);
                yield break;
            }

            var relayServerData = serverRelayUtilityTask.Result;

            // Display the joinCode to the user.

            Debug.Log("Starting host");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();
            yield return null;
        }

        /// <summary>
        /// Event handler for when a Join code is given.
        /// </summary>
        public void OnJoinCodeEntered()
        {
            Debug.Log("Player - Entering join code from host, typed in manually.");
            //var inputFieldComponent = joinCodeInput.GetComponent<Text>();
            //joinCode = inputFieldComponent.text;
            joinCode = joinCodeInput.text.ToString();
            Debug.Log("Join code entered: " + joinCode);
            joinCode = joinCode.Substring(0, 6); 
            Debug.Log("Join code entered: " + joinCode);
            
            UpdateUI();
        }

        /// <summary>
        /// Event handler for when the Join button is clicked.
        /// </summary>
        public async void OnJoin()
        {
            Debug.Log("Player - Joining host allocation using join code.");
            await JoinRelayServerFromJoinCode(joinCode);
            // try
            // {
            //     var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            //     playerAllocationId = joinAllocation.AllocationId;
            //     Debug.Log("Player Allocation ID: " + playerAllocationId);
            // }
            // catch (RelayServiceException ex)
            // {
            //     Debug.LogError(ex.Message + "\n" + ex.StackTrace);
            // }

            ConfigureTransportAndStartNgoAsConnectingPlayer();

            UpdateUI();
        }

        public async Task<RelayServerData> JoinRelayServerFromJoinCode(string joinedCode)
        {
            JoinAllocation allocation;
            try
            {
                allocation = await RelayService.Instance.JoinAllocationAsync(joinedCode);
                playerAllocationId = allocation.AllocationId;
                Debug.Log("Player Allocation ID: " + playerAllocationId);
            }
            catch
            {
                Debug.LogError("Relay create join code request failed");
                throw;
            }

            Debug.Log($"client: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
            Debug.Log($"host: {allocation.HostConnectionData[0]} {allocation.HostConnectionData[1]}");
            Debug.Log($"client: {allocation.AllocationId}");

            return new RelayServerData(allocation, "dtls");
        }

        IEnumerator ConfigureTransportAndStartNgoAsConnectingPlayer()
        {
            // Populate RelayJoinCode beforehand through the UI
            var clientRelayUtilityTask = JoinRelayServerFromJoinCode(joinCode);

            while (!clientRelayUtilityTask.IsCompleted)
            {
                yield return null;
            }

            if (clientRelayUtilityTask.IsFaulted)
            {
                Debug.LogError("Exception thrown when attempting to connect to Relay Server. Exception: " + clientRelayUtilityTask.Exception.Message);
                yield break;
            }

            var relayServerData = clientRelayUtilityTask.Result;

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
            yield return null;
        }

        void UpdateUI()
        {
            PlayerIdText.text = playerId.ToString();
            RegionsDropdown.interactable = regions.Count > 0;
            RegionsDropdown.options?.Clear();
            RegionsDropdown.AddOptions(new List<string> {autoSelectRegionName});  // index 0 is always auto-select (use QoS)
            RegionsDropdown.AddOptions(regionOptions);
            if (!String.IsNullOrEmpty(allocationRegion))
            {
                if (regionOptions.Count == 0)
                {
                    RegionsDropdown.AddOptions(new List<String>(new[] { allocationRegion }));
                }
                RegionsDropdown.value = RegionsDropdown.options.FindIndex(option => option.text == allocationRegion);
            }
            HostAllocationIdText.text = hostAllocationId.ToString();
            JoinCodeText.text = joinCode;
            PlayerAllocationIdText.text = playerAllocationId.ToString();
        }
        
        // void OnGUI()
        // {
        //     GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        //     if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        //     {
        //         StartButtons();
        //     }
        //     else
        //     {
        //         StatusLabels();

        //         SubmitNewPosition();
        //     }

        //     GUILayout.EndArea();
        // }

        // static void StartButtons()
        // {
        //     if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
        //     if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
        //     //if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
        // }

        // static void StatusLabels()
        // {
        //     var mode = NetworkManager.Singleton.IsHost ?
        //         "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

        //     GUILayout.Label("Transport: " +
        //         NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        //     GUILayout.Label("Mode: " + mode);
        // }

        // static void SubmitNewPosition()
        // {
        //     if (GUILayout.Button(NetworkManager.Singleton.IsServer ? "Move" : "Request Position Change"))
        //     {
        //         if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient )
        //         {
        //             foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds)
        //                 NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<NetworkPlayer>().Move();
        //         }
        //         else
        //         {
        //             var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        //             var player = playerObject.GetComponent<NetworkPlayer>();
        //             player.Move();
        //         }
        //     }
        // }
    }