using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Menu Navigation")]
    [SerializeField] private SaveSlotsMenu saveSlotsMenu;
    [Header("Menu Buttons")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button quitButton;
    
    private void Start()
    {
        DisableButtonsDependingOnData();
        GameManager.Instance.SetCursorDefault();
    }

    private void DisableButtonsDependingOnData()
    {
        // If there is no save data, disable the continue button
        if (!DataPersistenceManager.Instance.HasGameData())
        {
            continueButton.interactable = false;
            loadGameButton.interactable = false;
        }
    }
    public void ContinueGame()
    {
        // DisableMenuButtons();
        DataPersistenceManager.Instance.SaveGame();
        // Get the last scene the player was in
        var lastScene = DataPersistenceManager.Instance.GetLastScene();
        
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.ShowInventory();
        }
        
        ScenesManager.LoadScene(lastScene);
    }

    public void NewGame()
    {
        // DisableMenuButtons();
        saveSlotsMenu.ActivateMenu(false);
        DeactivateMenu();
    }
    
    public void LoadGame()
    {
        // DisableMenuButtons();
        saveSlotsMenu.ActivateMenu(true);
        DeactivateMenu();
    }
    
    public void ActivateMenu()
    {
        gameObject.SetActive(true);
        DisableButtonsDependingOnData();
    }
    
    public void DeactivateMenu()
    {
        gameObject.SetActive(false);
    }

    public void QuitGame()
    {
        // Quit the game
        Application.Quit();
    }
    
    // private void DisableMenuButtons()
    // {
    //     continueButton.interactable = false;
    //     loadGameButton.interactable = false;
    //     newGameButton.interactable = false;
    //     optionsButton.interactable = false;
    //     quitButton.interactable = false;
    // }
    //
    // private void EnableMenuButtons()
    // {
    //     continueButton.interactable = true;
    //     loadGameButton.interactable = true;
    //     newGameButton.interactable = true;
    //     optionsButton.interactable = true;
    //     quitButton.interactable = true;
    // }
}
