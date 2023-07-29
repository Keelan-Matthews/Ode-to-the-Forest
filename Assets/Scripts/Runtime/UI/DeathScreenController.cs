using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScreenController : MonoBehaviour
{
    [SerializeField] private GameObject deathScreen;
    // Start is called before the first frame update
    private void Awake()
    {
        Health.OnPlayerDeath += Health_OnPlayerDeath;
    }
    
    private void OnDestroy()
    {
        Health.OnPlayerDeath -= Health_OnPlayerDeath;
    }
    
    private void Health_OnPlayerDeath()
    {
        // Show the death screen
        deathScreen.SetActive(true);
        
        // Do a bunch of cool essence things
    }
    
    public void OnContinueButtonClicked()
    {
        GameManager.OnGameContinue();
    }
}
