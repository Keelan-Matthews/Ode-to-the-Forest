using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapTutorialTrigger : MonoBehaviour
{
    [SerializeField] private Dialogue minimapTutorialDialogue;

    private GameObject dialogueComponent;
    private DialogueController _dialogueController;

    private bool _spoken;
    
    private void Awake()
    {
        if (RoomController.Instance != null)
        {
            dialogueComponent = RoomController.Instance.dialogueComponent;
            if (dialogueComponent == null) return;
            _dialogueController = dialogueComponent.GetComponent<DialogueController>();
        }

        if (GameManager.Instance.HasSeenMinimapTutorial)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_spoken) return;
        if (!other.CompareTag("Player")) return;
        if (GameManager.Instance.isTutorial) return;
        if (_dialogueController == null) return;
        _spoken = true;
        _dialogueController.SetDialogue(minimapTutorialDialogue);
        dialogueComponent.SetActive(true);
        _dialogueController.StartDialogue();
        GameManager.Instance.HasSeenMinimapTutorial = true;
    }
}
