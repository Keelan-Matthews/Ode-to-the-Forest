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
    
    // Bob the icon up and down
    private void Update()
    {
        var position = icon.transform.position;
        position = new Vector3(position.x, position.y + Mathf.Sin(Time.time) * 0.001f, position.z);
        icon.transform.position = position;
    }
    
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
        
        // If it is an upgrade, play the upgrade sound and animation
        if (_abilityEffect.IsUpgrade())
        {
            AudioManager.PlaySound(AudioManager.Sound.ObeliskUseGood, transform.position);
            PlayerController.Instance.PlayUpgradeAnimation();
        }
        else // Otherwise, play the downgrade sound and animation
        {
            AudioManager.PlaySound(AudioManager.Sound.ObeliskUseBad, transform.position);
            PlayerController.Instance.PlayDowngradeAnimation();
        }

        // Add the ability to the list of purchased abilities if it is not already in the list
        if (!AbilityManager.Instance.GetPurchasedAbilities().Contains(_abilityEffect))
        {
            AbilityManager.Instance.PurchaseAbility(_abilityEffect);
        }
        
        // Remove the essence from the player
        PlayerController.Instance.SpendEssence(cost);
        
        Debug.Log("Player has been given the ability: " + _abilityEffect.name + ".");
        
        _used = true;
        
        // Disable the icon
        icon.SetActive(false);
        
        // Get the Interactable gameobject
        var interactable = GetComponentInChildren<Interactable>();
        // Set the interacted bool to true
        interactable.SetInteractable(false);
        
        AbilityManager.Instance.DisplayAbilityStats(_abilityEffect);
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
