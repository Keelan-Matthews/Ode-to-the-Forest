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
            _dialogueController.SetDialogueAudio("Collector");
            if (GameManager.Instance.HasSeenCollector) return;
            if (_isTalkingToCollector) return;
            _dialogueController.SetDialogue(tutorialDialogue);
            dialogueComponent.SetActive(true);
            _dialogueController.StartDialogue();
            _isTalkingToCollector = true;
        }
    }
    
    // Destroy the trigger when the player leaves the trigger
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance.HasSeenCollector) return;
            _dialogueController.StopDialogue();
            GameManager.Instance.HasSeenCollector = true;
            DataPersistenceManager.Instance.SaveGame(true);
            _isTalkingToCollector = false;
            Destroy(gameObject);
        }
    }
}
