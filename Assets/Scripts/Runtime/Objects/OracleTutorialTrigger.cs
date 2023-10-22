using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OracleTutorialTrigger : MonoBehaviour
{
    [SerializeField] private Dialogue tutorialDialogue;
    
    [SerializeField] private GameObject dialogueComponent;
    private DialogueController _dialogueController;
    
    private bool _isTalkingToOracle;
    
    private void Awake()
    {
        if (GameManager.Instance.HasSeenOracle)
        {
            Destroy(gameObject);
        }
        _dialogueController = dialogueComponent.GetComponent<DialogueController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance.HasSeenOracle) return;
            if (_isTalkingToOracle) return;
            _dialogueController.SetDialogue(tutorialDialogue);
            dialogueComponent.SetActive(true);
            _dialogueController.StartDialogue();
            _isTalkingToOracle = true;
            GameManager.Instance.HasSeenOracle = true;
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
            if (GameManager.Instance.HasSeenOracle) return;
            // _dialogueController.StopDialogue();
            GameManager.Instance.HasSeenOracle = true;
            _isTalkingToOracle = false;
            Destroy(gameObject);
        }
    }
}
