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

    private static readonly int Slash = Animator.StringToHash("Slash");

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
            permaSeedImage.enabled = true;
            permaSeedImage.sprite = permaSeed.icon;
        }

        // If it is not portal, halve the essence and display "You Died"
        // Else, display "You Escaped"
        if (!isPortal)
        {
            // Display "You Died"
            deathText.text = "You Withered";
            
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
                // Trigger the slash animation
                halvedEssenceCross.GetComponent<Animator>().SetTrigger(Slash);
                arrow.SetActive(true);
            }

            // Remove their perma seed if they have one
            if (PermaSeedManager.Instance.HasSeed())
            {
                PermaSeedManager.Instance.RemoveStoredPermaSeed();
                permaSeedCross.SetActive(true);
                permaSeedCross.GetComponent<Animator>().SetTrigger(Slash);
            }
        }
        else
        {
            // Make player invincible
            PlayerController.Instance.SetInvincible(true);
            // Display "You Escaped"
            deathText.text = "You Teleported";
            // Display the essence
            essenceText.text = PlayerController.Instance.essence.ToString();
        }
    }
    
    public void OnContinueButtonClicked()
    {
        // Unapply all the perma seed effects and abilities
        DataPersistenceManager.Instance.SaveGame();
        PermaSeedManager.Instance.UnapplyPermaSeedEffects();
        PlayerController.Instance.UnapplyAllAbilities();
        GameManager.OnGameContinue();
    }
}
