/******************************************************************************
 * Death menu script for players that die
 * Gives the player the choice to respawn
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DapperDino.Events.CustomEvents;

public class DeathMenu : MonoBehaviour
{
    // Boolean that checks if the menu is active or not
    public static bool activeMenu = false;

    public bool isDead = false;

    // Menu UI to be displayed
    public GameObject menuUI;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerDead() 
    {
        isDead = true;
        menuUI.SetActive(true);
    }
}
