using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeathScreenController : MonoBehaviour
{
    [Header("Game Object References")]
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private TextMeshProUGUI deathText;
    [SerializeField] private TextMeshProUGUI essenceText;
    [SerializeField] private Image permaSeedImage;
    [SerializeField] private GameObject permaSeedCross;

    [SerializeField] private TextMeshProUGUI halvedEssenceText;
    [SerializeField] private GameObject halvedEssenceCross;
    [SerializeField] private GameObject arrow;
    
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
        GameManager.Instance.SetCursorDefault();
        // Show the death screen
        deathScreen.SetActive(true);
        
        // add the perma seed image
        var permaSeed = PermaSeedManager.Instance.GetStoredPermaSeed();
        if (permaSeed != null)
        {
            permaSeedImage.sprite = permaSeed.icon;
        }

        // If it is not portal, halve the essence and display "You Died"
        // Else, display "You Escaped"
        if (!isPortal)
        {
            // Display "You Died"
            deathText.text = "You Died";
            
            essenceText.text = PlayerController.Instance.essence.ToString();
            // Halve the essence if it is greate than one, and round up
            if (PlayerController.Instance.essence > 1)
            {
                var halvedEssence = Mathf.CeilToInt(PlayerController.Instance.essence / 2f);
                PlayerController.Instance.essence = halvedEssence;
                halvedEssenceText.text = halvedEssence.ToString();
                // Display the halved essence
                halvedEssenceText.enabled = true;
            
                // Display the cross
                halvedEssenceCross.SetActive(true);
                arrow.SetActive(true);
            }

            // Remove their perma seed if they have one
            if (PermaSeedManager.Instance.HasSeed())
            {
                PermaSeedManager.Instance.RemoveStoredPermaSeed();
                permaSeedCross.SetActive(true);
            }
        }
        else
        {
            // Display "You Escaped"
            deathText.text = "You Ran";
            // Display the essence
            essenceText.text = PlayerController.Instance.essence.ToString();
        }
    }
    
    public void OnContinueButtonClicked()
    {
        GameManager.OnGameContinue();
    }
}
