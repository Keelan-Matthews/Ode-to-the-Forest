using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class HomeRoomController : MonoBehaviour, IDataPersistence
{
    public TextMeshProUGUI homeEssenceText;
    public static HomeRoomController Instance;
    private int _homeEssence;
    
    private void Awake()
    {
        Instance = this;
        
        // Subscribe to OnPlayerDeath
        // Health.OnPlayerDeath += Health_OnPlayerDeath;
    }

    private void Start()
    {
        var essence = PlayerController.Instance.GetEssence();
        _homeEssence += essence;
        homeEssenceText.enabled = false;
        homeEssenceText.text = _homeEssence.ToString();
        homeEssenceText.enabled = true;

        // Reset the player's essence
        PlayerController.Instance.ResetEssence();
        
        // Reset the player's abilities
        PlayerController.Instance.ResetAbilities();
        
        PlayerController.Instance.GoToSleep();
    }
    
    public int GetEssence()
    {
        return _homeEssence;
    }
    
    public void SpendEssence(int amount)
    {
        _homeEssence -= amount;
        homeEssenceText.enabled = false;
        homeEssenceText.text = _homeEssence.ToString();
        homeEssenceText.enabled = true;
    }
    
    public void LoadData(GameData data)
    {
        // Load the home essence
        _homeEssence = data.HomeEssence;
    }

    public void SaveData(GameData data)
    {
        // Save the home essence
        data.HomeEssence = _homeEssence;
    }

    public bool FirstLoad()
    {
        return true;
    }
}
