/******************************************************************************
 * Main menu script for the initial starting screen
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeMenu : MonoBehaviour
{
    // Boolean that checks if the menu is active or not
    public static bool activeMenu = false;

    // Menu UI to be displayed
    public GameObject menuUI;

    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (activeMenu)
            {
                Resume();
            } else {
                DisplayMenu();
            }
        }
    }

    // Resumes the activies of the game
    public void Resume()
    {
        menuUI.SetActive(false);
        Time.timeScale = 1f; // Shouldn't affect regardless of single or multiplayer
        activeMenu = false;
    }

    // Sets the Menu to be active and pauses the game if it is singleplayer, else activity continues
    void DisplayMenu()
    {
        menuUI.SetActive(true);
        Time.timeScale = 0f; // Freezes the game but should only occur if in Singleplayer
        activeMenu = true;
    }

    // Returns the player to the main menu screen
    public void LoadMenu()
    {
        Time.timeScale = 1f; // Returns the time back to normal if the player chooses to leave the game and return to the menu
        SceneManager.LoadScene("Start Menu");
    }

    // Quits the application for the player
    public void Quit()
    {
        Application.Quit();
    }
}
