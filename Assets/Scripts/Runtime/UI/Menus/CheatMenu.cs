using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Runtime.Abilities;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class CheatMenu : MonoBehaviour
{
    // This will take an input from a text field and see if it matches any of the valid cheat codes
    // If it does, it will execute the cheat code
    // It has an animation state for when the cheat code is invalid and valid
    
    [SerializeField] private Animator animator;
    // Text mesh pro input field
    [SerializeField] private TMP_InputField cheatCodeInput;
    private string[] abilityNames;
    private string sceneName;
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
    }

    public void TestCode()
    {
        // Reset the trigger
        animator.ResetTrigger(ValidCheat);
        animator.ResetTrigger(InvalidCheat);
        
        var upperCaseInput = cheatCodeInput.text.ToUpper();
        cheatCodeInput.text = string.Empty;
        switch (upperCaseInput)
        {
            case "GODMODEON":
                if (sceneName != "ForestMain") break;
                PlayerController.Instance.SetInvincible(true);
                animator.SetTrigger(ValidCheat);
                AudioManager.PlaySound(AudioManager.Sound.SeedGrown, transform.position);
                return;
            case "GODMODEOFF":
                if (sceneName != "ForestMain") break;
                PlayerController.Instance.SetInvincible(false);
                animator.SetTrigger(ValidCheat);
                AudioManager.PlaySound(AudioManager.Sound.SeedGrown, transform.position);
                return;
            case "KACHING":
                // Add 20 essence to the player
                if (sceneName == "ForestMain")
                {
                    PlayerController.Instance.AddFullEssence(20);
                    animator.ResetTrigger(ValidCheat);
                    AudioManager.PlaySound(AudioManager.Sound.SeedGrown, transform.position);
                }
                else if (sceneName == "Home")
                {
                    HomeRoomController.Instance.AddEssence(20);
                    animator.ResetTrigger(ValidCheat);
                    AudioManager.PlaySound(AudioManager.Sound.SeedGrown, transform.position);
                }
                else break;
                return;
            case "REVEALMAP":
                if (sceneName != "ForestMain") break;
                if (MinimapManager.Instance)
                {
                    MinimapManager.Instance.UpdateAllRooms();
                    animator.ResetTrigger(ValidCheat);
                    AudioManager.PlaySound(AudioManager.Sound.SeedGrown, transform.position);
                }
                else break;
                return;
            case "ALLABILITIES":
                if (AbilityManager.Instance)
                {
                    AbilityManager.Instance.PurchaseAllAbilities();
                    animator.ResetTrigger(ValidCheat);
                    AudioManager.PlaySound(AudioManager.Sound.SeedGrown, transform.position);
                }
                else break;
                return;
            default:
                TestAbilityCode(upperCaseInput);
                return;
        }
        
        // If we get here, we didn't find a match
        // Play the invalid animation
        animator.SetTrigger(InvalidCheat);
        AudioManager.PlaySound(AudioManager.Sound.DisabledButtonClick, transform.position);
    }
    
    private void TestAbilityCode(string input)
    {
        if (abilityNames == null)
        {
            animator.ResetTrigger(InvalidCheat);
            AudioManager.PlaySound(AudioManager.Sound.DisabledButtonClick, transform.position);
            return;
        }
        
        foreach (var abilityName in abilityNames)
        {
            if (input == abilityName.ToUpper())
            {
                var ability = AbilityManager.Instance.GetAbility(abilityName);
                PlayerController.Instance.AddAbility(ability);
                AbilityManager.Instance.TriggerAbilityDisplay(ability);
                animator.ResetTrigger(ValidCheat);
                AudioManager.PlaySound(AudioManager.Sound.SeedGrown, transform.position);
                return;
            }
            
            if (input == $"ADD{abilityName.ToUpper()}")
            {
                if (PermaSeedManager.Instance.HasSeed())
                {
                    // Drop the old seed
                    var oldSeedName = PermaSeedManager.Instance.GetStoredPermaSeed().seedName;
                    GameManager.Instance.DropSpecificPermaSeed(PlayerController.Instance.transform.position, oldSeedName);
                    PermaSeedManager.Instance.RemoveStoredPermaSeed();
                }
                
                var permaSeed = PermaSeedManager.Instance.GetSpecificPermaSeed(abilityName);
                PermaSeedManager.Instance.AddPermaSeed(permaSeed);

                AudioManager.PlaySound(AudioManager.Sound.SeedPickup, transform.position);
                animator.ResetTrigger(ValidCheat);
                AudioManager.PlaySound(AudioManager.Sound.SeedGrown, transform.position);
                return;
            }
        }

        // If we get here, we didn't find a match
        // Play the invalid animation
        animator.ResetTrigger(InvalidCheat);
        AudioManager.PlaySound(AudioManager.Sound.DisabledButtonClick, transform.position);
    }
}
