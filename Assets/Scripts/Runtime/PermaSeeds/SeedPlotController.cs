using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

public class SeedPlotController : MonoBehaviour, IDataPersistence
{
    private bool _isPlanted;
    private bool _isGrown;
    public bool isLocked = true;
    [SerializeField] private bool isMiniMapSeedPlot;
    private PermaSeed _permaSeed;
    
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private GameObject tutorialArrow;
    [SerializeField] private ConfirmationPopupMenu confirmationPopupMenu;
    
    [Header("Ability information references")]
    [SerializeField] private GameObject abilityInformation;
    [SerializeField] private TextMeshProUGUI abilityName;
    [SerializeField] private TextMeshProUGUI abilityDescription;
    [SerializeField] private Image abilityIcon;
    
    public int seedPlotIndex;
    [SerializeField] private int costToUnlock = 1;

    [Header("Plot sprites")]
    [SerializeField] private SpriteRenderer plotSpriteRenderer;
    [SerializeField] private Sprite lockedPlotSprite;
    [SerializeField] private Sprite unlockedPlotSprite;

    private bool fadingIn;
    private bool fadingOut;
    
    [Header("Particle emitters")]
    [SerializeField] private ParticleSystem unlockPlotParticleEmitter;
    [SerializeField] private ParticleSystem plantSeedParticleEmitter;
    [SerializeField] private ParticleSystem growSeedParticleEmitter;
    
    private Interactable _interactable;
    [SerializeField] private Animator seedAnimator;
    private static readonly int UnlockPlot = Animator.StringToHash("unlockPlot");

    private void Awake()
    {
        _interactable = GetComponentInChildren<Interactable>();
        _interactable.SetCost(costToUnlock);
    }
    
    private void Start()
    {
        // If this is the minimap plot and the tutorial has been completed, then destroy the tutorial arrow
        if (isMiniMapSeedPlot && !GameManager.Instance.isTutorial)
        {
            Destroy(tutorialArrow);
        }
        
        // If it is a minimap seed plot, unlock it
        if (isMiniMapSeedPlot)
        {
            Unlock(false);
        }
        
        // _interactable.SetInteractable(!isLocked);
        // _interactable.SetInteractable(true);
        
        abilityInformation.SetActive(false);
    }
    
    public void Unlock(bool playSound = true)
    {
        isLocked = false;
        plotSpriteRenderer.sprite = unlockedPlotSprite;
        // _interactable.SetInteractable(false);
        
        _interactable = GetComponentInChildren<Interactable>();
        _interactable.SetPromptText("Plant");
        _interactable.SetCost(0);
        
        if (playSound)
        {
            AudioManager.PlaySound(AudioManager.Sound.PlotUnlocked, transform.position);
            
            // Trigger the unlock plot particle emitter
            unlockPlotParticleEmitter.Play();
        }
    }
    
    public void Interact()
    {
        if (!_interactable.IsInteractable()) return;
        if (isLocked)
        {
            // See if they have enough essence to unlock it in the home room
            if (HomeRoomController.Instance.GetEssence() < costToUnlock)
            {
                _interactable.TriggerCannotAfford();
                Debug.Log("Player has " + HomeRoomController.Instance.GetEssence() + " essence, but needs " + costToUnlock + " to unlock a seed plot.");
                return;
            }
            
            // Unlock the plot
            Unlock();
            Debug.Log("Player has unlocked a seed plot.");
            
            // Remove the essence from home essence
            HomeRoomController.Instance.SpendEssence(costToUnlock);
        } 
        else if (!_isPlanted)
        {
            // Check if the player has a permaSeed
            if (!PermaSeedManager.Instance.HasSeed())
            {
                Debug.Log("Player has no seed to plant.");
                
                // Hide the interact prompt
                _interactable.TriggerCannotAfford();
                return;
            }
            
            // Don't allow the player to plant the SeedOfLife
            if (PermaSeedManager.Instance.GetStoredPermaSeed().seedName == "SeedOfLife")
            {
                _interactable.TriggerCannotAfford();
                return;
            }

            Plant(PermaSeedManager.Instance.PlantSeed(seedPlotIndex));
            
            // Update interactable prompt text
            _interactable.SetPromptText("Grow");
            var essenceRequired = _permaSeed.essenceRequired;
            _interactable.SetCost(essenceRequired);
        }
        else if (!_isGrown)
        {
            // Try grow the plant
            if (_permaSeed.Grow(HomeRoomController.Instance.GetEssence()))
            {
                // Subtract essenceRequired from the home Essence
                HomeRoomController.Instance.SpendEssence(_permaSeed.essenceRequired);
                
                Grow();

                if (!isMiniMapSeedPlot) return;
                dialogueController.gameObject.SetActive(true);
                dialogueController.SetDialogue(dialogue);
                Destroy(tutorialArrow);
            
                GameManager.Instance.isTutorial = false;
                DataPersistenceManager.Instance.SaveGame();
                dialogueController.StartDialogue();
                
                _interactable.DisableInteraction();
            }
            else
            {
                // If it hasn't grown, do nothing
                _interactable.TriggerCannotAfford();
                Debug.Log("Player has " + PlayerController.Instance.GetEssence() + " essence, but needs " + _permaSeed.essenceRequired + " to grow a seed.");
            }
        }
        else
        {
            // If its the minimap seed plot, then cannot uproot it
            if (isMiniMapSeedPlot)
            {
                Debug.Log("Player cannot uproot the minimap seed plot.");
                _interactable.DisableInteraction();
                return;
            }

            // Show the confirmation popup menu
            confirmationPopupMenu.ActivateMenu(
                "Are you sure you want to uproot this seed? You will need to find it again.",
                () =>
                {
                    Uproot();
                    
                    _interactable.SetPromptText("Plant");
                    _interactable.SetCost(0);
                },
                () =>
                {
                    Debug.Log("Player has cancelled uprooting the seed.");
                    
                    // Hide the interact prompt
                    _interactable.DisableInteraction();
                });
        }
    }

    private void Plant(PermaSeed permaSeed, bool playSound = true)
    {
        // Plant the seed in the plot
        _permaSeed = permaSeed;
        _isPlanted = true;
        
        // Update the runtime animator controller
        seedAnimator.runtimeAnimatorController = _permaSeed.animatorController;
        seedAnimator.SetTrigger("PlantSeed");
        
        _interactable = GetComponentInChildren<Interactable>();
        _interactable.SetPromptText("Grow");
        var essenceRequired = _permaSeed.essenceRequired;
        _interactable.SetCost(essenceRequired);
        
        // Add the seed to activePermaSeeds
        PermaSeedManager.Instance.AddActiveSeed(_permaSeed);
        
        if (!playSound) return;
        AudioManager.PlaySound(AudioManager.Sound.SeedPlanted, transform.position);
        
        // Trigger the plant seed particle emitter
        plantSeedParticleEmitter.Play();
    }
    
    private void Grow(bool playSound = true)
    {
        _isGrown = true;
        
        _permaSeed.SetIsGrown(true);

        seedAnimator.SetTrigger("GrowSeed");
        
        // Update the interactable prompt text
        _interactable = GetComponentInChildren<Interactable>();
        _interactable.SetPromptText("Uproot");
        _interactable.SetCost(0);

        if (playSound)
        {
            AudioManager.PlaySound(AudioManager.Sound.SeedGrown, transform.position);
            
            growSeedParticleEmitter.Play();
        }
        
        DisplayAbilityStats();

        // Make it not interactable if it is the minimap seed
        if (!isMiniMapSeedPlot) return;
        
        _interactable = GetComponentInChildren<Interactable>();
        // Set the interacted bool to true
        _interactable.SetInteractable(false);
    }
    
    private void Uproot(bool playSound = true)
    {
        // Uproot the seed
        _isPlanted = false;
        _isGrown = false;
            
        // Remove the seed from activePermaSeeds
        PermaSeedManager.Instance.UprootSeed(_permaSeed);
        
        // Play the uproot animation
        seedAnimator.SetTrigger("UprootSeed");
        
        // Update the interactable prompt text
        _interactable.SetPromptText("Plant");
        _interactable.SetCost(0);
            
        _permaSeed = null;
            
        if (playSound)
        {
            AudioManager.PlaySound(AudioManager.Sound.SeedUproot, transform.position);
            
            plantSeedParticleEmitter.Play();
        }
    }
    
    public void DisplayAbilityStats()
    {
        // If there is no permaSeed or it is not grown, do nothing
        if (_permaSeed == null || !_isGrown) return;
        abilityInformation.SetActive(false);
        abilityInformation.SetActive(true);
        AudioManager.PlaySound(AudioManager.Sound.ShowMenu, transform.position);

        var abilityEffect = _permaSeed.GetAbilityEffect();
        
        abilityName.text = abilityEffect.abilityName;
        
        abilityDescription.text = abilityEffect.description;
        
        abilityIcon.sprite = abilityEffect.icon;
        
        // Force canvas update
        Canvas.ForceUpdateCanvases();
        var layoutGroups = abilityInformation.GetComponentsInChildren<HorizontalLayoutGroup>();
        // Disable the layout groups and then re-enable them to force the text to wrap
        foreach (var layoutGroup in layoutGroups)
        {
            layoutGroup.enabled = false;
            layoutGroup.enabled = true;
        }
        
        abilityInformation.GetComponent<Animator>().SetTrigger("Show");
    }

    public void DisableAbilityInformation()
    {
        abilityInformation.SetActive(false);
        abilityInformation.GetComponent<Animator>().SetTrigger("Hide");
    }

    public void SaveData(GameData data)
    {
        if (_permaSeed != null)
        {
            data.SeedPlotSeeds[seedPlotIndex] = _permaSeed.seedName;
        }
        else
        {
            data.SeedPlotSeeds[seedPlotIndex] = "";
        }
        
        data.GrownSeeds[seedPlotIndex] = _isGrown;
        data.UnlockedPlots[seedPlotIndex] = !isLocked;
    }
    
    public void LoadData(GameData data)
    {
        // Get the animator in the child
        seedAnimator = GetComponentInChildren<Animator>();
        isLocked = !data.UnlockedPlots[seedPlotIndex];
        var tempSeed = PermaSeedManager.Instance.GetSpecificPermaSeed(data.SeedPlotSeeds[seedPlotIndex]);
        _isPlanted = tempSeed != null;
        _isGrown = data.GrownSeeds[seedPlotIndex];
        
        // If it is unlocked, unlock it
        if (!isLocked)
        {
            Unlock(false);
        }
        
        // trigger the correct animation
        if (!_isPlanted) return;
        if (_isGrown)
        {
            Plant(tempSeed, false);
            Grow(false);
        }
        else
        {
            Plant(tempSeed, false);
        }
    }

    public bool FirstLoad()
    {
        return true;
    }
}
