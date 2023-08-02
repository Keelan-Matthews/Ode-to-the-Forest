using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public int seedPlotIndex;
    
    [Header("Plot sprites")]
    [SerializeField] private SpriteRenderer plotSpriteRenderer;
    [SerializeField] private Sprite lockedPlotSprite;
    [SerializeField] private Sprite unlockedPlotSprite;
    
    private Interactable _interactable;
    [SerializeField] private Animator seedAnimator;
    private static readonly int UnlockPlot = Animator.StringToHash("unlockPlot");
    private static readonly int PlantSeed = Animator.StringToHash("PlantSeed");
    private static readonly int GrowSeed = Animator.StringToHash("GrowSeed");
    private static readonly int UprootSeed = Animator.StringToHash("UprootSeed");

    private void Start()
    {
        _interactable = GetComponentInChildren<Interactable>();

        // If this is the minimap plot and the tutorial has been completed, then destroy the tutorial arrow
        if (isMiniMapSeedPlot && !GameManager.Instance.isTutorial)
        {
            Destroy(tutorialArrow);
        }
        
        // If it is a minimap seed plot, unlock it
        if (isMiniMapSeedPlot)
        {
            Unlock();
        }
        
        _interactable.SetInteracted(!isLocked);
    }
    
    public void Unlock()
    {
        isLocked = false;
        plotSpriteRenderer.sprite = unlockedPlotSprite;
        _interactable.SetInteracted(false);
    }
    
    public void Interact()
    {
        if (isLocked) return;
        
        if (!_isPlanted)
        {
            // Check if the player has a permaSeed
            if (!PermaSeedManager.Instance.HasSeed())
            {
                Debug.Log("Player has no seed to plant.");
                return;
            }

            Plant();
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
            }
            else
            {
                // If it hasn't grown, do nothing
                Debug.Log("Player has " + PlayerController.Instance.GetEssence() + " essence, but needs " + _permaSeed.essenceRequired + " to grow a seed.");
            }
        }
        else
        {
            // If its the minimap seed plot, then cannot uproot it
            if (isMiniMapSeedPlot)
            {
                Debug.Log("Player cannot uproot the minimap seed plot.");
                return;
            }
            // Some kind of "Are you sure?" prompt

            Uproot();
        }
    }

    private void Plant()
    {
        // Plant the seed in the plot
        _permaSeed = PermaSeedManager.Instance.PlantSeed(seedPlotIndex);
        _isPlanted = true;
        
        seedAnimator.SetTrigger(PlantSeed);
        
        AudioManager.PlaySound(AudioManager.Sound.SeedPlanted, transform.position);
    }
    
    private void Grow()
    {
        _isGrown = true;
        
        // Add the seed to activePermaSeeds
        PermaSeedManager.Instance.AddActiveSeed(_permaSeed);

        seedAnimator.SetTrigger(GrowSeed);
        
        // Make it not interactable if it is the minimap seed
        if (!isMiniMapSeedPlot) return;
        
        // Set the interacted bool to true
        _interactable.SetInteracted(true);
        
        AudioManager.PlaySound(AudioManager.Sound.SeedGrown, transform.position);
    }
    
    private void Uproot()
    {
        // Uproot the seed
        _isPlanted = false;
        _isGrown = false;
            
        // Remove the seed from activePermaSeeds
        PermaSeedManager.Instance.UprootSeed(_permaSeed);
        
        // Play the uproot animation
        seedAnimator.SetTrigger(UprootSeed);
            
        _permaSeed = null;
            
        Debug.Log("Player has uprooted a seed.");
    }

    public void SaveData(GameData data)
    {
        data.SeedPlotSeeds[seedPlotIndex] = _permaSeed;
        data.GrownSeeds[seedPlotIndex] = _isGrown;
        data.UnlockedPlots[seedPlotIndex] = !isLocked;
    }
    
    public void LoadData(GameData data)
    {
        // Get the animator in the child
        seedAnimator = GetComponentInChildren<Animator>();
        _permaSeed = data.SeedPlotSeeds[seedPlotIndex];
        isLocked = !data.UnlockedPlots[seedPlotIndex];
        _isPlanted = _permaSeed != null;
        _isGrown = data.GrownSeeds[seedPlotIndex];
        
        // trigger the correct animation
        if (!_isPlanted) return;
        if (_isGrown)
        {
            Plant();
            Grow();
        }
        else
        {
            Plant();
        }
        
        // If it is unlocked, unlock it
        if (!isLocked)
        {
            Unlock();
        }
        
        // Set the interacted bool to true if unlocked, false if locked
        _interactable.SetInteracted(!isLocked);
    }

    public bool FirstLoad()
    {
        return true;
    }
}
