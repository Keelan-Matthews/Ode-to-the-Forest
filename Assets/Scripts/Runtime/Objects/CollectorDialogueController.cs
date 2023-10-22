using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectorDialogueController : MonoBehaviour
{
    [SerializeField] private Dialogue collectorDialogue;

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

    public void TalkToCollector()
    {
        if (_isTalkingToCollector) return;
        _dialogueController.isPaused = false;
        
        _dialogueController.isIntermittent = true;
        _dialogueController.IsRandom = true;
        _dialogueController.SetDialogue(collectorDialogue);
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
        var interactable = GetComponentInChildren<Interactable>();
        interactable.HidePromptText();
    }
    
    public void StopTalkingToCollector()
    {
        // _dialogueController.StopDialogue();
        _isTalkingToCollector = false;
    }
}
