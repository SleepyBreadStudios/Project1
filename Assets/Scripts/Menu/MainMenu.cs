/******************************************************************************
 * Main menu script for the initial starting screen
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class MainMenu : MonoBehaviour
{

    // First main menu of game
    public GameObject menuUI;

    // Second part of main menu
    public GameObject credits;

    // // Menu handling hosting and joining
    // public GameObject menuUI3;

    // Sends the player to the game
    public void PlayGame()
    {
        SceneManager.LoadScene("Overworld");
    }

    // Exits the application for the player
    public void QuitGame() 
    {
        Application.Quit();
    }

    // Activates the menu where players can select single vs multiplayer
    public void PlayerSelection()
    {
        menuUI.SetActive(false);
        //menuUI2.SetActive(true);
    }

    // // Returns the player to the previous screen
    public void Back()
    {
        menuUI.SetActive(true);
        credits.SetActive(false);
    }

    // Activates the hosting and client side
    public void Multiplayer()
    {
        //menuUI2.SetActive(false);
        //menuUI3.SetActive(true);
    }

    // Sends the player back to the credits screen
    public void Credits()
    {
        menuUI.SetActive(false);
        credits.SetActive(true); 
    }
}
