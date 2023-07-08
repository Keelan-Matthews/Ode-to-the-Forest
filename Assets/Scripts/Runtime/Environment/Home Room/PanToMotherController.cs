using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanToMotherController : MonoBehaviour
{
    public GameObject dialogueComponent;
    private DialogueController _dialogueController;
    
    // Start is called before the first frame update
    void Start()
    {
        _dialogueController = dialogueComponent.GetComponent<DialogueController>();
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            // Set the camera to follow the player
            CameraController.Instance.panToMother = true;
            
            // Set the game object to active
            dialogueComponent.SetActive(true);
            
            // Start the dialogue
            _dialogueController.StartDialogue();
        }
    }
    
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            // Set the camera to follow the player
            CameraController.Instance.panToMother = false;
            
            // Stop the dialogue
            _dialogueController.StopDialogue();
        }
    }
}
