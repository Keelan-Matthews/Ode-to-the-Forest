using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class HomeRoomController : MonoBehaviour
{
    public TextMeshProUGUI homeEssenceText;
    public static HomeRoomController Instance;
    public HomeStats homeStats;
    
    private void Awake()
    {
        Instance = this;
        
        // Subscribe to OnPlayerDeath
        // Health.OnPlayerDeath += Health_OnPlayerDeath;
    }

    private void Start()
    {
        var essence = PlayerController.Instance.GetEssence();
        homeStats.homeEssence += essence;
        homeEssenceText.enabled = false;
        homeEssenceText.text = homeStats.homeEssence.ToString();
        homeEssenceText.enabled = true;

        // Reset the player's essence
        PlayerController.Instance.ResetEssence();
        
        // Reset the player's abilities
        PlayerController.Instance.ResetAbilities();
    }
    
    public int GetEssence()
    {
        return homeStats.homeEssence;
    }
    
    public void SpendEssence(int amount)
    {
        homeStats.homeEssence -= amount;
        homeEssenceText.enabled = false;
        homeEssenceText.text = homeStats.homeEssence.ToString();
        homeEssenceText.enabled = true;
    }

    // private void Health_OnPlayerDeath()
    // {
    //     var essence = PlayerController.Instance.GetEssence();
    //     homeStats.homeEssence += essence;
    //     homeEssenceText.enabled = false;
    //     homeEssenceText.text = homeStats.homeEssence.ToString();
    //     homeEssenceText.enabled = true;
    //
    //     // Reset the player's essence
    //     PlayerController.Instance.ResetEssence();
    // }
}
