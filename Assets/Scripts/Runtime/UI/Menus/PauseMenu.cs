using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    
    public GameObject pauseMenuUI;

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
}
