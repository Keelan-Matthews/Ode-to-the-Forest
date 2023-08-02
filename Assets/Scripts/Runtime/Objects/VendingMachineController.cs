using Runtime.Abilities;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class VendingMachineController : MonoBehaviour
{
    // This script is responsible for giving abilities to the player.
    // When nearby, the player can press the interact button to buy an ability.
    // The player can only buy one ability at a time.
    // The player can only buy an ability if they have enough money.
    // The ability is given at random and is automatically equipped.
    
    [SerializeField] private int cost = 3;
    public Light2D obeliskLight;
    [SerializeField] private SunlightController sunlightController;
    [SerializeField] private AudioSource obeliskHum;
    private Animator _animator;

    private bool _used = false;
    private static readonly int IsUpgrade = Animator.StringToHash("IsUpgrade");
    private static readonly int IsDowngrade = Animator.StringToHash("IsDowngrade");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
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
        
        // Get a random ability from the ability list
        var ability = AbilityManager.Instance.GetRandomAbility();
        
        // If it is the replenish health ability but the player is already at full health, get a different ability
        while (ability.name == "Replenish Health" && PlayerController.Instance.GetComponent<Health>().HealthValue == PlayerController.Instance.GetComponent<Health>().MaxHealth)
        {
            Debug.Log("Player is already at full health, getting a different ability.");
            ability = AbilityManager.Instance.GetRandomAbility();
        }

        var isUpgrade = ability.IsUpgrade();
        // Give the player the ability
        PlayerController.Instance.AddAbility(ability);
        
        // Remove the essence from the player
        PlayerController.Instance.SpendEssence(cost);
        
        Debug.Log("Player has been given the ability: " + ability.name + ".");
        
        // Trigger the animation
        if (isUpgrade)
        {
            // Play the upgrade sound
            AudioManager.PlaySound(AudioManager.Sound.ObeliskUseGood, transform.position);
            _animator.SetBool(IsUpgrade, true);
            
            // Update the lights
            sunlightController.LightRoomUpgradeObelisk();
            
            // Play the player upgrade animation
            PlayerController.Instance.PlayUpgradeAnimation();
        }
        else
        {
            AudioManager.PlaySound(AudioManager.Sound.ObeliskUseBad, transform.position);
            _animator.SetBool(IsDowngrade, true);
            
            // Update the lights
            sunlightController.LightRoomDowngradeObelisk();
            
            // Change obelisk light to red
            obeliskLight.color = new Color(0.9f, 0.4f, 0.3f);
            
            // Play the player downgrade animation
            PlayerController.Instance.PlayDowngradeAnimation();
        }
        
        // Trigger the room growth animation
        GameManager.Instance.activeRoom.GrowBackground();
        
        // Stop playing the hum sound
        obeliskHum.Stop();

        _used = true;
        
        // Get the Interactable gameobject
        var interactable = GetComponentInChildren<Interactable>();
        // Set the interacted bool to true
        interactable.SetInteractable(false);
        
        AbilityManager.Instance.DisplayAbilityStats(ability);
    }
}
