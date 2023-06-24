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

    public void Update()
    {
        //code.text = UIManager.UI.joinCode.text;
        joinCode.text = UIManager.UI.joinCode.text;
    }
}
