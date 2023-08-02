using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeathScreenController : MonoBehaviour
{
    [Header("Game Object References")]
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private TextMeshProUGUI deathText;
    [SerializeField] private TextMeshProUGUI essenceText;
    
    public static DeathScreenController Instance;
    // Start is called before the first frame update
    private void Awake()
    {
        // Instance
        Instance = this;
        
        // On death, trigger the screen
        Health.OnPlayerDeath += Health_OnPlayerDeath; 
    }

    private void Health_OnPlayerDeath()
    {
        TriggerScreen(false);
    }

    private void OnDestroy()
    {
        Health.OnPlayerDeath -= Health_OnPlayerDeath;
    }
    
    public void TriggerScreen(bool isPortal)
    {
        // Show the death screen
        deathScreen.SetActive(true);
        
        // If it is not portal, halve the essence and display "You Died"
        // Else, display "You Escaped"
        if (!isPortal)
        {
            // Display "You Died"
            deathText.text = "You Died";
            // Halve the essence
            var essence = PlayerController.Instance.essence / 2;
            essenceText.text = essence.ToString();
        }
        else
        {
            // Display "You Escaped"
            deathText.text = "You Escaped";
            // Display the essence
            essenceText.text = PlayerController.Instance.essence.ToString();
        }
    }
    
    public void OnContinueButtonClicked()
    {
        GameManager.OnGameContinue();
    }
}
