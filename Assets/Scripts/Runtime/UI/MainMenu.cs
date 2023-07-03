using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // Load the home base
        ScenesManager.LoadScene("Home");
    }
    
    public void LoadOptions()
    {
        // Load the options menu
        // ScenesManager.LoadScene("Options");
    }
    

    public void QuitGame()
    {
        // Quit the game
        Application.Quit();
    }
}
