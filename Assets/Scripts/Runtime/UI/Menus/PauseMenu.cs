using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    
    public GameObject pauseMenuUI;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button cheatsButton;
    [SerializeField] private ConfirmationPopupMenu confirmationPopupMenu;
    [SerializeField]private GameObject settingsMenu;
    [SerializeField] private GameObject cheatsMenu;
    [SerializeField] private GameObject mainMenu;

    // Update is called once per frame
    private void Update()
    {
        if (!GameManager.Instance || !ScenesManager.Instance) return;
        if (ScenesManager.Instance.currentSceneName == "Credits" || ScenesManager.Instance.currentSceneName == "MainMenu" || GameManager.Instance.AlmanacOpen) return;
        
        // If Tilda key is pressed, toggle the cheat button visibility
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            cheatsButton.gameObject.SetActive(!cheatsButton.gameObject.activeSelf);
        }
        
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
        
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
        cheatsMenu.SetActive(false);
        
        // Set cursor to default
        GameManager.Instance.SetCursorDefault();
    }
    
    public void QuitGame()
    {
        // Show the confirmation popup menu
        confirmationPopupMenu.ActivateMenu(
            "Are you sure you want to quit the game? Any unsaved progress will be lost.",
            () =>
            {
                GameIsPaused = false;
                // Quit the game
                Application.Quit();
            },
            () =>
            {
                // Hide the confirmation popup menu
                confirmationPopupMenu.DeactivateMenu();
            });
    }

    public void Save()
    {
        DataPersistenceManager.Instance.SaveGame();
    }

    public void LoadMenu()
    {
        // Show the confirmation popup menu
        confirmationPopupMenu.ActivateMenu(
            "Are you sure you want to load the main menu? Any unsaved progress will be lost.",
            () =>
            {
                // Load the main menu
                Time.timeScale = 1f;
                
                if (PermaSeedManager.Instance != null && ScenesManager.Instance.currentSceneName == "ForestMain" && PermaSeedManager.Instance.HasSeed())
                {
                    PermaSeedManager.Instance.RemoveStoredPermaSeed();
                }
                
                // If there is an InventoryManager, hide ti
                if (InventoryManager.Instance != null)
                {
                    InventoryManager.Instance.HideInventory();
                }
                
                GameIsPaused = false;

                ScenesManager.LoadScene("MainMenu");
            },
            () =>
            {
                // Hide the confirmation popup menu
                confirmationPopupMenu.DeactivateMenu();
            });
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
