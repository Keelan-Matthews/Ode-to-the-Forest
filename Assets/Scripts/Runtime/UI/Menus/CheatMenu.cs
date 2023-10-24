using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Runtime.Abilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CheatMenu : MonoBehaviour
{
    // This will take an input from a text field and see if it matches any of the valid cheat codes
    // If it does, it will execute the cheat code
    // It has an animation state for when the cheat code is invalid and valid
    
    [SerializeField] private Animator animator;
    // Text mesh pro input field
    [SerializeField] private TMP_InputField cheatCodeInput;
    [SerializeField] private GameObject invincibleButton;
    [SerializeField] private GameObject addAbilityButton;
    [SerializeField] private GameObject addPermaSeedButton;
    [SerializeField] private TMP_Dropdown abilityDropdown;
    public Sprite selectedButton;
    public Sprite unselectedButton;
    private string[] abilityNames;
    private string sceneName;
    private bool _invincibilityOn;
    private static readonly int ValidCheat = Animator.StringToHash("ValidCheat");
    private static readonly int InvalidCheat = Animator.StringToHash("InvalidCheat");

    private void Start()
    {
        if (AbilityManager.Instance)
        {
            var abilities = AbilityManager.Instance.GetAbilityNames();
            abilityNames = abilities.ToArray();
        }

        sceneName = ScenesManager.Instance.currentSceneName;
        
        // Set the dropdown options
        abilityDropdown.ClearOptions();
        
        // Make the first option "Select an ability"
        abilityNames[0] = "SELECT ABILITY";
        abilityDropdown.AddOptions(abilityNames.ToList());
        
        // Add three options at the bottom called "Clairvoyance", "Fortune Cookie", and "Marker"
        abilityDropdown.options.Add(new TMP_Dropdown.OptionData("Clairvoyance"));
        abilityDropdown.options.Add(new TMP_Dropdown.OptionData("Marker"));
        
        // Add an option at the bottom called "Unlock all abilities"
        abilityDropdown.options.Add(new TMP_Dropdown.OptionData("Unlock all abilities"));
        
        // Set the dropdown value to 0
        abilityDropdown.value = 0;
    }
    
    // On scene loaded
    private void OnEnable()
    {
        sceneName = ScenesManager.Instance.currentSceneName;
        abilityDropdown.value = 0;

        if (sceneName != "ForestMain")
        {
            // Disable the invincible button
            invincibleButton.GetComponent<Button>().interactable = false;
            addAbilityButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            invincibleButton.GetComponent<Button>().interactable = true;
            addAbilityButton.GetComponent<Button>().interactable = true;
        }
        
        // If the 0 ability is selected, disable the add ability button
        if (abilityDropdown.value == 0)
        {
            addAbilityButton.GetComponent<Button>().interactable = false;
            addPermaSeedButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            if (sceneName == "ForestMain")
            {
                addAbilityButton.GetComponent<Button>().interactable = true;
            }
            
            addPermaSeedButton.GetComponent<Button>().interactable = true;
        }
    }

    public void ToggleInvincibility()
    {
        if (sceneName != "ForestMain")
        {
            AudioManager.PlaySound(AudioManager.Sound.DisabledButtonClick, transform.position);
            return;
        }
        _invincibilityOn = !_invincibilityOn;
        PlayerController.Instance.SetInvincible(_invincibilityOn);
        AudioManager.PlaySound(AudioManager.Sound.SeedGrown, transform.position);
        
        if (_invincibilityOn)
        {
            invincibleButton.GetComponentInChildren<TextMeshProUGUI>().text = "INVINCIBLE: ON";
            invincibleButton.GetComponent<Image>().sprite = selectedButton;
        }
        else
        {
            invincibleButton.GetComponentInChildren<TextMeshProUGUI>().text = "INVINCIBLE: OFF";
            invincibleButton.GetComponent<Image>().sprite = unselectedButton;
        }
    }
    
    public void AddEssence()
    {
        if (sceneName == "ForestMain")
        {
            PlayerController.Instance.AddFullEssence(5);
            AudioManager.PlaySound(AudioManager.Sound.SeedGrown, transform.position);
        }
        else if (sceneName == "Home")
        {
            HomeRoomController.Instance.AddEssence(5);
            AudioManager.PlaySound(AudioManager.Sound.SeedGrown, transform.position);
        }
    }

    public void UpdateDropdownButtons()
    {
        if (abilityDropdown.value >= abilityNames.Length || !PermaSeedManager.Instance.GetSpecificPermaSeed(abilityNames[abilityDropdown.value]))
        {
            addPermaSeedButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            addPermaSeedButton.GetComponent<Button>().interactable = true;
        }
        
        // If nothing is selected, disable the button
        if (abilityDropdown.value == 0 || sceneName != "ForestMain")
        {
            addAbilityButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            addAbilityButton.GetComponent<Button>().interactable = true;
        }
    }
    
    public void AddAbility()
    {
        if (abilityDropdown.value == 0) return;

        string abilityName;
        if (abilityDropdown.value >= abilityNames.Length)
        {
            abilityName = abilityDropdown.options[abilityDropdown.value].text;
        }
        else
        {
            abilityName = abilityNames[abilityDropdown.value];
        }

        if (abilityName == "Unlock all abilities")
        {
            AbilityManager.Instance.PurchaseAllAbilities();
        }
        else
        {
            AbilityEffect ability;
            if (abilityName is "Clairvoyance" or "Fortune Cookie" or "Marker")
            {
                ability = AbilityManager.Instance.GetOracleAbility(abilityName);
            }
            else
            {
                ability = AbilityManager.Instance.GetAbility(abilityName, true);
            }
            
            PlayerController.Instance.AddAbility(ability);
            AbilityManager.Instance.TriggerAbilityDisplay(ability);
        }
        
        AudioManager.PlaySound(AudioManager.Sound.SeedGrown, transform.position);
    }
    
    public void AddPermaSeed()
    {
        if (abilityDropdown.value == 0) return;
        var abilityName = abilityNames[abilityDropdown.value];
        
        // If the ability is not a perma seed, return
        if (!PermaSeedManager.Instance.GetSpecificPermaSeed(abilityName))
        {
            AudioManager.PlaySound(AudioManager.Sound.DisabledButtonClick, transform.position);
            return;
        }

        if (PermaSeedManager.Instance.HasSeed())
        {
            // Drop the old seed
            var oldSeedName = PermaSeedManager.Instance.GetStoredPermaSeed().seedName;
            GameManager.Instance.DropSpecificPermaSeed(PlayerController.Instance.transform.position, oldSeedName);
            PermaSeedManager.Instance.RemoveStoredPermaSeed();
        }
                
        var permaSeed = PermaSeedManager.Instance.GetSpecificPermaSeed(abilityName);
        PermaSeedManager.Instance.AddPermaSeed(permaSeed);
        AudioManager.PlaySound(AudioManager.Sound.SeedGrown, transform.position);
    }
}
