using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSlotsMenu : MonoBehaviour
{
    [Header("Menu Navigation")] [SerializeField]
    private MainMenu mainMenu;

    private SaveSlot[] _saveSlots;
    private bool _isLoadingGame = false;

    private void Awake()
    {
        // Get all save slots
        _saveSlots = GetComponentsInChildren<SaveSlot>();
    }

    public void OnSaveSlotClicked(SaveSlot saveSlot)
    {
        DataPersistenceManager.Instance.ChangeSelectedProfileId(saveSlot.GetProfileId());

        if (_isLoadingGame)
        {
            // Load the home base
            ScenesManager.LoadScene("Home");
        }
        else
        {
            // Load the tutorial
            DataPersistenceManager.Instance.NewGame();
            ScenesManager.LoadScene("Tutorial");
        }
    }

    public void GoBack()
    {
        mainMenu.ActivateMenu();
        DeactivateMenu();
    }

    public void ActivateMenu(bool isLoadingGame)
    {
        gameObject.SetActive(true);

        _isLoadingGame = isLoadingGame;

        var profilesGameData = DataPersistenceManager.Instance.GetAllProfilesGameData();

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
}