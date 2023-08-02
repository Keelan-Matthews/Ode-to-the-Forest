using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanToMotherController : MonoBehaviour
{
    public GameObject dialogueComponent;
    private DialogueController _dialogueController;
    public Dialogue loreDialogue;
    private Interactable _interactable;
    
    // Start is called before the first frame update
    void Start()
    {
        _dialogueController = dialogueComponent.GetComponent<DialogueController>();
        _interactable = GetComponentInChildren<Interactable>();
    }
    
    public void PanToMother()
    {
        if (GameManager.Instance.isTutorial)
        {
            _interactable.SetInteractable(false);
            return;
        }
        
        _interactable.SetInteractable(true);
        CameraController.Instance.panToMother = true;
    }
    
    public void TalkToMother()
    {
        if (GameManager.Instance.isTutorial) return;
        _dialogueController.SetDialogue(loreDialogue);
        dialogueComponent.SetActive(true);
        _dialogueController.StartDialogue();
    }
    
    public void PanAwayFromMother()
    {
        // Set the camera to follow the player
        CameraController.Instance.panToMother = false;
            
        // Stop the dialogue
        _dialogueController.StopDialogue();
    }
}
