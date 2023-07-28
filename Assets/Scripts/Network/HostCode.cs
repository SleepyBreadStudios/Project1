/******************************************************************************
 * TESTING: Host code for the players to see and use to join the game
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HostCode : MonoBehaviour
{
    [SerializeField]
    public Text joinCode;

    // [SerializeField] private VoidEvent onEscUpdated = null;

    // public Action OnEscUpdated = delegate { };
    // // Boolean that checks if the menu is active or not
    // public static bool activeMenu = false;

    // public bool playerMenusNotOpen = true;

    // // Menu UI to be displayed
    // public GameObject menuUI;

    // void Awake()
    // {
    //     OnEscUpdated += onEscUpdated.Raise;
    // }

    public void Update()
    {
        //code.text = UIManager.UI.joinCode.text;
        //joinCode.text = UIManager.UI.joinCode.text;
        joinCode.text = RelayManager.relay.joinCode;
        if (joinCode.text == "n/a")
        {
            joinCode.text = "";
        }
        // if (Input.GetKeyDown(KeyCode.Escape) && playerMenusNotOpen)
        // {
        //     if (activeMenu)
        //     {
        //         Resume();
        //     }
        //     else
        //     {
        //         DisplayMenu();
        //     }
        // }
    }
    
}
