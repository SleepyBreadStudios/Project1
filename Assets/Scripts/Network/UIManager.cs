/******************************************************************************
 * UI manager, handles clicking of buttons.
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/

using DilmerGames.Core.Singletons;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    public static UIManager UI;
    [SerializeField]
    private Button startServerButton;

    [SerializeField]
    private Button startHostButton;

    [SerializeField]
    private Button startClientButton;

    [SerializeField]
    private TextMeshProUGUI playersInGameText;

    [SerializeField]
    private TextMeshProUGUI objectCount;

    [SerializeField]
    private TextMeshProUGUI enemyCount;

    [SerializeField]
    private TMP_InputField joinCodeInput;

    [SerializeField]
    public Text joinCode;

    // [SerializeField]
    // private Button executePhysicsButton;


    //private bool hasServerStarted;

    private void Awake()
    {
        Cursor.visible = true;
        int width = 768; // or something else
        int height= 432; // or something else
        bool isFullScreen = false; // should be windowed to run in arbitrary resolution
        //int desiredFPS = 60; // or something else
    
        Screen.SetResolution (width , height, isFullScreen);

        if (UI == null)
        {
            UI = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // playersInGameText.text = $"Players in game: {PlayersManager.Instance.PlayersInGame}";
        // objectCount.text = $"ObjectCount: {GameObject.FindGameObjectsWithTag("Item").Length}";
        // enemyCount.text = $"Enemy Count: {GameObject.FindGameObjectsWithTag("Enemy").Length}";
        // if(GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        // {
        //     enemyCount.text = $"You win!";
        // }
    }

    void Start()
    {
        // START SERVER
        startServerButton.onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton.StartServer())
                Debug.Log("Server started...");
            else
                Debug.Log("Unable to start server...");
        });

        // START HOST
        startHostButton?.onClick.AddListener(async () =>
        {
            //Debug.Log("hello");
            // this allows the UnityMultiplayer and UnityMultiplayerRelay scene to work with and without
            // relay features - if the Unity transport is found and is relay protocol then we redirect all the 
            // traffic through the relay, else it just uses a LAN type (UNET) communication.
            if (RelayManager.Instance.IsRelayEnabled) 
                SceneManager.LoadSceneAsync("Overworld");
                await RelayManager.Instance.SetupRelay();

            if (NetworkManager.Singleton.StartHost()) {
                Debug.Log("Host started...");
                joinCode.text = RelayManager.Instance.joinCode; // Allows the join code to be displayed
            }
            else
                Debug.Log("Unable to start host...");
        });

        // START CLIENT
        startClientButton?.onClick.AddListener(async () =>
        {
            if (RelayManager.Instance.IsRelayEnabled && !string.IsNullOrEmpty(joinCodeInput.text))
                await RelayManager.Instance.JoinRelay(joinCodeInput.text);

            if(NetworkManager.Singleton.StartClient())
                Debug.Log("Client started...");
            else
                Debug.Log("Unable to start client...");
        });

        // STATUS TYPE CALLBACKS
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            Debug.Log($"{id} just connected...");
        };

        //NetworkManager.Singleton.OnServerStarted += () =>
        //{
        //    hasServerStarted = true;
        //};

        // executePhysicsButton.onClick.AddListener(() => 
        // {
        //     if (!hasServerStarted)
        //     {
        //         Debug.Log("Server has not started...");
        //         return;
        //     }
        //     SpawnerControl.Instance.SpawnObjects();
        // });
    }
}