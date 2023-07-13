using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
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
        DataPersistenceManager.Instance.NewGame();
        ScenesManager.LoadScene("Tutorial");
    }

    public void QuitGame()
    {
        // Quit the game
        Application.Quit();
    }
}
