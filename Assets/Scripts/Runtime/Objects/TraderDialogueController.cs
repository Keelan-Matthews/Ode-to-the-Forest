using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraderDialogueController : MonoBehaviour
{
   
    [SerializeField] private Dialogue traderDialogue;

    private GameObject dialogueComponent;
    private DialogueController _dialogueController;
    
    private bool _isTalkingToTrader;

    private void Awake()
    {
        dialogueComponent = RoomController.Instance.dialogueComponent;
        _dialogueController = dialogueComponent.GetComponent<DialogueController>();
    }

    private void Update()
    {
        _isTalkingToTrader = GameManager.Instance.activeDialogue;
    }

    public void TalkToTrader()
    {
        if (_isTalkingToTrader) return;
        _dialogueController.isIntermittent = true;
        _dialogueController.IsRandom = true;
        _dialogueController.SetDialogue(traderDialogue);
        dialogueComponent.SetActive(true);

        if (_dialogueController.isPaused)
        {
            _dialogueController.ResumeDialogue();
        }
        else
        {
            _dialogueController.StartDialogue();
        }
        
        _isTalkingToTrader = true;
    }
    
    public void StopTalkingToTrader()
    {
        _dialogueController.StopDialogue();
        _isTalkingToTrader = false;
    }
}
