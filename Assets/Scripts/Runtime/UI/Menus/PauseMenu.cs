using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    
    public GameObject pauseMenuUI;
    [SerializeField] private Button saveButton;

    // Update is called once per frame
    private void Update()
    {
        if (ScenesManager.Instance.currentSceneName == "MainMenu") return;
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        // If the game is paused, resume the game
        if (GameIsPaused)
        {
            Resume();
        }
        // If the game is not paused, pause the game
        else
        {
            Pause();
        }
    }
    
    public void Resume()
    {
        // Hide the pause menu
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
    
    public void Pause()
    {
        // Show the pause menu
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        
        // If the player is in the home room, enable the save button, else disable it
        if (ScenesManager.Instance.currentSceneName == "Home")
        {
            EnableSaveButton();
        }
        else
        {
            DisableSaveButton();
        }
    }
    
    public void QuitGame()
    {
        // Quit the game
        Application.Quit();
    }

    public void Save()
    {
        DataPersistenceManager.Instance.SaveGame();
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        // Load the main menu
        ScenesManager.LoadScene("MainMenu");
    }
    
    public void DisableSaveButton()
    {
        // Disable the pause menu
        saveButton.interactable = false;
    }
    
    public void EnableSaveButton()
    {
        // Enable the pause menu
        saveButton.interactable = true;
    }
}
