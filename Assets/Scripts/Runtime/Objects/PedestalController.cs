using System;
using System.Collections;
using System.Collections.Generic;
using Runtime.Abilities;
using UnityEngine;

public class PedestalController : MonoBehaviour
{
    private AbilityEffect _abilityEffect;
    [SerializeField] private int cost = 1;
    [SerializeField] private GameObject icon;
    private bool _used;
    
    public void Interact()
    {
        if (_used) return;
        // Check if the player has enough essence
        if (PlayerController.Instance.GetEssence() < cost)
        {
            Debug.Log("Player has " + PlayerController.Instance.GetEssence() + " essence, but needs " + cost + " to buy an ability.");
            return;
        }

        // Give the player the ability
        PlayerController.Instance.AddAbility(_abilityEffect);
        
        // Add the ability to the list of purchased abilities if it is not already in the list
        if (!AbilityManager.Instance.GetPurchasedAbilities().Contains(_abilityEffect))
        {
            AbilityManager.Instance.PurchaseAbility(_abilityEffect);
        }
        
        // Remove the essence from the player
        PlayerController.Instance.SpendEssence(cost);
        
        Debug.Log("Player has been given the ability: " + _abilityEffect.name + ".");
        
        _used = true;
    }
    
    public void SetAbilityEffect(AbilityEffect abilityEffect)
    {
        _abilityEffect = abilityEffect;
        
        if (AbilityManager.Instance.GetPurchasedAbilities().Contains(_abilityEffect))
        {
            // Update the icon to show that the ability has been purchased before
            icon.GetComponent<SpriteRenderer>().sprite = _abilityEffect.icon;
        }
    }
    
    // Get the name of the ability
    public string GetAbilityName()
    {
        return _abilityEffect.name;
    }
}
