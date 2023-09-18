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

    private void Awake()
    {
        // Get the interactable component
        var interactable = GetComponentInChildren<Interactable>();
        if (interactable == null) return;
        
        // Set the prompt text
        interactable.SetCost(cost);
    }
    
    private void OnEnable()
    {
        GameManager.OnDiscount += SetCost;
    }
    
    private void OnDisable()
    {
        GameManager.OnDiscount -= SetCost;
    }

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
        // Get the Interactable gameobject
        var interactable = GetComponentInChildren<Interactable>();
        
        if (!interactable.IsInteractable()) return;
        
        // Check if the player has enough essence
        if (GameManager.Instance.IsSellYourSoul)
        {
            if (PlayerController.Instance.GetHealth() < 2)
            {
                interactable.TriggerCannotAfford();
                return;
            }
        }
        else
        {
            if (PlayerController.Instance.GetEssence() < cost)
            {
                interactable.TriggerCannotAfford();
                return;
            }
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

        AbilityManager.Instance.PurchaseAbility(_abilityEffect);
        
        // Remove the essence from the player
        if (GameManager.Instance.IsSellYourSoul && _abilityEffect.abilityName != "Glass Cannon")
        {
            var confirmationMenu = RoomController.Instance.confirmationPopupMenu;
            confirmationMenu.ActivateMenu(
                "Buying this will cause you to wither. Are you sure you want to continue?",
                () =>
                {
                    // Decrease the player's health by 1
                    PlayerController.Instance.GetComponent<Health>().TakeDamage(2);
                },
                () =>
                {
                    _used = false;
                });

            if (!_used) return;
        }
        else
        {
            // Remove the essence from the player
            PlayerController.Instance.SpendEssence(cost);
        }
        
        _used = true;
        
        // Disable the icon
        icon.SetActive(false);
        // Set the interacted bool to true
        interactable.SetInteractable(false);
        interactable.DisableInteraction();
        
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
    
    public void SetCost(int discount)
    {
        cost -= discount;
        var interactable = GetComponentInChildren<Interactable>();
        interactable.SetCost(cost);
    }
    
    // Get the name of the ability
    public string GetAbilityName()
    {
        return _abilityEffect.name;
    }
}
