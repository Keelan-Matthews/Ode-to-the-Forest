using System.Collections;
using System.Collections.Generic;
using Runtime.Abilities;
using UnityEngine;

public class ShrineOfYouthController : MonoBehaviour
{
    private int _numFountainsActivated;
    private Animator _animator;
    [SerializeField] private int cost;
    [SerializeField] private SunlightController sunlightController;
    [SerializeField] private AudioSource hum;
    [SerializeField] private Room room;
    [SerializeField] private AbilityEffect abilityEffect;
    private bool _used;
    private static readonly int On1 = Animator.StringToHash("On1");
    private static readonly int On2 = Animator.StringToHash("On2");
    private static readonly int On3 = Animator.StringToHash("On3");
    private static readonly int On4 = Animator.StringToHash("On4");

    private void Start()
    {
        FountainController.OnFountainActivated += IncrementFountainsActivated;
        GetComponentInChildren<Interactable>().SetInteractable(false);
    }
    
    private void OnEnable()
    {
        GameManager.OnDiscount += SetCost;
    }
    
    private void OnDisable()
    {
        GameManager.OnDiscount -= SetCost;
    }
    
    private void OnDestroy()
    {
        FountainController.OnFountainActivated -= IncrementFountainsActivated;
    }

    private void IncrementFountainsActivated()
    {
        _animator = GetComponentsInChildren<Animator>()[0];
        _numFountainsActivated++;

        switch (_numFountainsActivated)
        {
            case 1:
                _animator.SetTrigger(On1);
                break;
            case 2:
                _animator.SetTrigger(On2);
                break;
            case 3:
                _animator.SetTrigger(On3);
                break;
            case 4:
                _animator.SetTrigger(On4);
                GetComponentInChildren<Interactable>().SetInteractable(true);
                // Trigger the room growth animation
                room.GrowBackground();
                // Stop playing the hum sound
                hum.Play();
                break;
            default:
                break;
        }
    }

    public void Interact()
    {
        if (_used) return;
        var interactable = GetComponentInChildren<Interactable>();
        
        if (GameManager.Instance.IsSellYourSoul)
        {
            if (PlayerController.Instance.GetHealth() <= 1)
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
        
        if (GameManager.Instance.IsSellYourSoul)
        {
            // Decrease the player's health by 1
            PlayerController.Instance.GetComponent<Health>().TakeDamage(1);
        }
        else
        {
            // Remove the essence from the player
            PlayerController.Instance.SpendEssence(cost);
        }
        
        AudioManager.PlaySound(AudioManager.Sound.ObeliskUseGood, transform.position);
        
        AbilityManager.Instance.PurchaseAbility(abilityEffect);

        // Update the lights
        sunlightController.LightRoomUpgradeObelisk();
            
        // Play the player upgrade animation
        PlayerController.Instance.PlayUpgradeAnimation();

        interactable.SetInteractable(false);
        interactable.DisableInteraction();
        _used = true;
        AbilityManager.Instance.DisplayAbilityStats(abilityEffect);
        CameraController.Instance.GetComponentInParent<CameraShake>().ShakeCamera(0.3f);
    }
    
    public void SetCost(int discount)
    {
        cost -= discount;
        var interactable = GetComponentInChildren<Interactable>();
        interactable.SetCost(cost);
    }
}
