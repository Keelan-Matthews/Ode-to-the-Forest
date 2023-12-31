using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectorController : MonoBehaviour
{
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private Dialogue noSeedDialogue;
    private GameObject dialogueComponent;
    private DialogueController _dialogueController;
    private bool _isTalkingToCollector;
    
    private void Awake()
    {
        dialogueComponent = RoomController.Instance.dialogueComponent;
        _dialogueController = dialogueComponent.GetComponent<DialogueController>();
    }
    
    private void Update()
    {
        _isTalkingToCollector = GameManager.Instance.activeDialogue;
    }
    
    public void Interact()
    {
        if (_isTalkingToCollector) return;
        if (!GameManager.Instance.HasSeenCollector) return;
        _dialogueController.isIntermittent = true;
        _dialogueController.IsRandom = true;
        
        // If the player doesn't have a perma seed, don't let them talk to the collector
        if (PermaSeedManager.Instance.HasSeed())
        {
            // Get the seed
            var seed = PermaSeedManager.Instance.GetStoredPermaSeed();
        
            // Get the cost of the seed
            var cost = seed.essenceRequired;
        
            // Remove the seed from the player's inventory and give them cost * 1/3 essence
            PermaSeedManager.Instance.RemoveStoredPermaSeed(true);

            var costRoundedUp = Mathf.CeilToInt(cost / 3);
            if (GameManager.Instance.IsSellYourSoul)
            {
                PlayerController.Instance.GetComponent<Health>().Heal(costRoundedUp);
            }
            else
            {
                PlayerController.Instance.AddFullEssence(costRoundedUp);
            }
            
            AudioManager.PlaySound(AudioManager.Sound.SeedGrown, transform.position);
            _dialogueController.isPaused = false;
            _dialogueController.SetDialogue(dialogue);
        }
        else
        {
            _dialogueController.isPaused = false;
            _dialogueController.SetDialogue(noSeedDialogue);
        }

        
        dialogueComponent.SetActive(true);

        if (_dialogueController.isPaused)
        {
            _dialogueController.ResumeDialogue();
        }
        else
        {
            _dialogueController.StartDialogue();
        }
        
        _isTalkingToCollector = true;
    }

    public void StopTalkingToCollector()
    {
        // _dialogueController.StopDialogue();
        _isTalkingToCollector = false;
    }
}
