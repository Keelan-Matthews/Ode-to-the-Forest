using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradertutorialTrigger : MonoBehaviour
{
    [SerializeField] private Dialogue tutorialDialogue;
    
    private GameObject dialogueComponent;
    private DialogueController _dialogueController;
    
    private bool _isTalkingToTrader;
    
    private void Awake()
    {
        if (GameManager.Instance.HasSeenTrader)
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
            if (_isTalkingToTrader) return;
            _dialogueController.SetDialogue(tutorialDialogue);
            dialogueComponent.SetActive(true);
            _dialogueController.StartDialogue();
            _isTalkingToTrader = true;
        }
    }
    
    // Destroy the trigger when the player leaves the trigger
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _dialogueController.StopDialogue();
            GameManager.Instance.HasSeenTrader = true;
            _isTalkingToTrader = false;
            Destroy(gameObject);
        }
    }
}
