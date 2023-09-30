using System.Collections;
using System.Collections.Generic;
using Runtime.Abilities;
using UnityEngine;

public class ShrineOfYouthController : MonoBehaviour, IDataPersistence
{
    private int _numFountainsActivated;
    [SerializeField] private Animator _animator;
    [SerializeField] private int cost;
    [SerializeField] private SunlightController sunlightController;
    [SerializeField] private AudioSource hum;
    private bool _used;
    private static readonly int On1 = Animator.StringToHash("On1");
    private static readonly int On2 = Animator.StringToHash("On2");
    private static readonly int On3 = Animator.StringToHash("On3");
    private static readonly int On4 = Animator.StringToHash("On4");

    private void Awake()
    {
        FountainController.OnFountainActivated += IncrementFountainsActivated;
        GetComponentInChildren<Interactable>().SetInteractable(false);
    }

    private void IncrementFountainsActivated()
    {
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
                GameManager.Instance.activeRoom.GrowBackground();
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

        var ability = AbilityManager.Instance.GetAbility("Vital-renew");
        AbilityManager.Instance.PurchaseAbility(ability);

        // Update the lights
        sunlightController.LightRoomUpgradeObelisk();
            
        // Play the player upgrade animation
        PlayerController.Instance.PlayUpgradeAnimation();
        
        // Stop playing the hum sound
        hum.Stop();
        
        interactable.SetInteractable(false);
        interactable.DisableInteraction();
        _used = true;
        AbilityManager.Instance.DisplayAbilityStats(ability);
        CameraController.Instance.GetComponentInParent<CameraShake>().ShakeCamera(0.3f);
    }

    public void LoadData(GameData data)
    {
        // In data.fountainsActivated, count the number of true values
        foreach (var fountainActivated in data.fountainActivated)
        {
            if (fountainActivated)
            {
                IncrementFountainsActivated();
            }
        }
    }

    public void SaveData(GameData data)
    {
    }

    public bool FirstLoad()
    {
        return true;
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }
}
