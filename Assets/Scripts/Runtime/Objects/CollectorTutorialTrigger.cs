using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectorTutorialTrigger : MonoBehaviour
{
    [SerializeField] private Dialogue tutorialDialogue;
    
    private GameObject dialogueComponent;
    private DialogueController _dialogueController;
    
    private bool _isTalkingToCollector;
    
    private void Awake()
    {
        if (GameManager.Instance.HasSeenCollector)
        {
            Destroy(gameObject);
        }
        dialogueComponent = RoomController.Instance.dialogueComponent;
        _dialogueController = dialogueComponent.GetComponent<DialogueController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance.HasSeenCollector) return;
            if (_isTalkingToCollector) return;
            _dialogueController.SetDialogue(tutorialDialogue);
            _dialogueController.isIntermittent = false;
            _dialogueController.isPaused = false;
            _dialogueController.IsRandom = false;
            dialogueComponent.SetActive(true);
            _dialogueController.StartDialogue();
            _isTalkingToCollector = true;
            var interactable = GetComponentInChildren<Interactable>();
            if (interactable == null) return;
            interactable.HidePromptText();
        }
    }
    
    // Destroy the trigger when the player leaves the trigger
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance.HasSeenCollector) return;
            // _dialogueController.StopDialogue();
            GameManager.Instance.HasSeenCollector = true;
            _isTalkingToCollector = false;
            Destroy(gameObject);
        }
    }
}
