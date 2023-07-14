using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotsMenu : MonoBehaviour
{
    [Header("Menu Navigation")] [SerializeField]
    private MainMenu mainMenu;
    
    [Header("Menu Buttons")]
    [SerializeField] private Button backButton;
    
    [Header("Confirmation Popup Menu")]
    [SerializeField] private ConfirmationPopupMenu confirmationPopupMenu;

    private SaveSlot[] _saveSlots;
    private bool _isLoadingGame = false;

    private void Awake()
    {
        // Get all save slots
        _saveSlots = GetComponentsInChildren<SaveSlot>();
    }

    public void OnSaveSlotClicked(SaveSlot saveSlot)
    {
        DisableMenuButtons();
            
        //case - loading game
        if (_isLoadingGame)
        {
            DataPersistenceManager.Instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
            SaveGameAndLoadScene("Home");
        }
        //case - new game
        else if (saveSlot.hasData)
        {
            confirmationPopupMenu.ActivateMenu(
                "Starting a new game will overwrite the existing save data. Are you sure you want to continue?",
                () =>
                {
                    DataPersistenceManager.Instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
                    DataPersistenceManager.Instance.NewGame();
                    SaveGameAndLoadScene("Tutorial");
                },
                () =>
                {
                    ActivateMenu(_isLoadingGame);
                });
        }
        // case - new game and no data
        else
        {
            DataPersistenceManager.Instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
            DataPersistenceManager.Instance.NewGame();
            SaveGameAndLoadScene("Tutorial");
        }
    }

    public void GoBack()
    {
        mainMenu.ActivateMenu();
        DeactivateMenu();
    }

    private void SaveGameAndLoadScene(string scene)
    {
        DataPersistenceManager.Instance.SaveGame();
        // Load the home base
        ScenesManager.LoadScene(scene);
    }
    
    public void ClearSaveSlot(SaveSlot saveSlot)
    {
        DisableMenuButtons();
        confirmationPopupMenu.ActivateMenu(
            "Are you sure you want to delete this save data?",
            () =>
            {
                DataPersistenceManager.Instance.DeleteProfileData(saveSlot.GetProfileId());
                ActivateMenu(_isLoadingGame);
            },
            () =>
            {
                ActivateMenu(_isLoadingGame);
            });
    }

    public void ActivateMenu(bool isLoadingGame)
    {
        gameObject.SetActive(true);

        _isLoadingGame = isLoadingGame;

        var profilesGameData = DataPersistenceManager.Instance.GetAllProfilesGameData();

        // Ensure back button is interactable
        backButton.interactable = true;
        
        // Loop through each save slot and set the data
        foreach (var saveSlot in _saveSlots)
        {
            // Get the profile id
            GameData profileData = null;
            profilesGameData.TryGetValue(saveSlot.GetProfileId(), out profileData);
            saveSlot.SetData(profileData);

            if (profileData == null && isLoadingGame)
            {
                saveSlot.SetInteractable(false);
            }
            else
            {
                saveSlot.SetInteractable(true);
            }
        }
    }

    public void DeactivateMenu()
    {
        gameObject.SetActive(false);
    }
    
    private void DisableMenuButtons()
    {
        foreach (var saveSlot in _saveSlots)
        {
            saveSlot.SetInteractable(false);
        }
        
        backButton.interactable = false;
    }
}