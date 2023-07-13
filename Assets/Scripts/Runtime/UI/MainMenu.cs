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
    
    private void Start()
    {
        // If there is no save data, disable the continue button
        if (!DataPersistenceManager.Instance.HasGameData())
        {
            continueButton.interactable = false;
        }
    }
    public void PlayGame()
    {
        // Load the home base
        ScenesManager.LoadScene("Home");
    }

    public void NewGame()
    {
        saveSlotsMenu.ActivateMenu(false);
        DeactivateMenu();
    }
    
    public void LoadGame()
    {
        saveSlotsMenu.ActivateMenu(true);
        DeactivateMenu();
    }
    
    public void ActivateMenu()
    {
        gameObject.SetActive(true);
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
}
