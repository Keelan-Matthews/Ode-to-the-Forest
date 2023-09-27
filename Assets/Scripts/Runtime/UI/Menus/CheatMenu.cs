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
        animator.ResetTrigger("ValidCheat");
        animator.ResetTrigger("InvalidCheat");
        
        var upperCaseInput = cheatCodeInput.text.ToUpper();
        cheatCodeInput.text = string.Empty;
        switch (upperCaseInput)
        {
            case "GODMODE":
                Debug.Log("God mode activated");
                animator.SetTrigger(ValidCheat);
                break;
            case "KACHING":
                Debug.Log("Kaching activated");
                animator.SetTrigger("ValidCheat");
                break;
            case "REVEALMAP":
                animator.SetTrigger("ValidCheat");
                break;
            case "SMDUNGEON":
                animator.SetTrigger("ValidCheat");
                break;
            case "XLDUNGEON":
                animator.SetTrigger("ValidCheat");
                break;
            case "UNLOCKALLABILITIES":
                animator.SetTrigger("ValidCheat");
                break;
            default:
                TestAbilityCode(upperCaseInput);
                break;
        }
    }
    
    private void TestAbilityCode(string input)
    {
        if (abilityNames == null)
        {
            animator.SetTrigger("InvalidCheat");
            return;
        }
        
        foreach (var abilityName in abilityNames)
        {
            if (input == abilityName.ToUpper())
            {
                // We have a match!
                // Get the ability from the ability manager
                // var ability = AbilityManager.Instance.GetAbilityByName(abilityName);
                // // Add it to the player's inventory
                // AbilityManager.Instance.AddAbilityToInventory(ability);
                // Play the animation
                animator.SetTrigger("ValidCheat");
                return;
            }
            
            if (input == $"ADD{abilityName.ToUpper()}")
            {
                // We have a match!
                // Get the ability from the ability manager
                // var ability = AbilityManager.Instance.GetAbilityByName(abilityName);
                // // Add it to the player's inventory
                // AbilityManager.Instance.AddAbilityToInventory(ability);
                // Play the animation
                animator.SetTrigger("ValidCheat");
                return;
            }
        }

        // If we get here, we didn't find a match
        // Play the invalid animation
        animator.SetTrigger("InvalidCheat");
    }
}
