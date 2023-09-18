using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraderDialogueController : MonoBehaviour
{
    [SerializeField] private Dialogue tutorialDialogue;
    [SerializeField] private Dialogue traderDialogue;

    private GameObject dialogueComponent;
    private DialogueController _dialogueController;
    
    private bool _isTalkingToTrader;

    private void Awake()
    {
        dialogueComponent = RoomController.Instance.dialogueComponent;
        _dialogueController = dialogueComponent.GetComponent<DialogueController>();
    }
    
    public void TalkToTrader()
    {
        if (_isTalkingToTrader) return;
        if (!GameManager.Instance.HasSeenTrader)
        {
            _dialogueController.SetDialogue(tutorialDialogue);
        }
        else
        {
            _dialogueController.SetDialogue(traderDialogue);
        }
        dialogueComponent.SetActive(true);
        _dialogueController.StartDialogue();
        _isTalkingToTrader = true;
    }
    
    public void StopTalkingToTrader()
    {
        _dialogueController.StopDialogue();
        _isTalkingToTrader = false;
    }
}
