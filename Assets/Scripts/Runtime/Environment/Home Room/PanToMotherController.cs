using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanToMotherController : MonoBehaviour
{
    public GameObject dialogueComponent;
    private DialogueController _dialogueController;
    public Dialogue loreDialogue;
    public Dialogue randomDialogue;
    public Dialogue endDialogue;
    private Interactable _interactable;
    private bool _madeInteractable;
    private bool _isTalkingToMother;
    [SerializeField] private GameObject arrow;

    // Start is called before the first frame update
    void Start()
    {
        _dialogueController = dialogueComponent.GetComponent<DialogueController>();
        _interactable = GetComponentInChildren<Interactable>();
        
        if (GameManager.Instance.isTutorial || GameManager.Instance.gameFinished)
        {
            _interactable.SetInteractable(false);
        }
    }
    
    public void PanToMother()
    {
        CameraController.Instance.panToMother = true;
        
        // If the player has the Seed of Life, update the prompt to say "Give Seed of Life"
        if (PermaSeedManager.Instance.HasSeed("Seed Of Life"))
        {
            _interactable.SetPromptText("Give Seed of Life");
        }
        else
        {
            _interactable.SetPromptText("Talk to Mother");
        }
    }

    private void Update()
    {
        if (GameManager.Instance.isTutorial || _madeInteractable) return;
        _interactable.SetInteractable(true);
        _madeInteractable = true;
    }

    public void TalkToMother()
    {
        if (GameManager.Instance.isTutorial || _isTalkingToMother) return;

        // If the player has the Seed Of Life, remove it from the inventory and play the end dialogue
        if (PermaSeedManager.Instance.HasSeed("Seed Of Life"))
        {
            PermaSeedManager.Instance.PlantSeed(0);
            _dialogueController.SetDialogue(endDialogue);
            _dialogueController.isPaused = false;
            _dialogueController.isIntermittent = false;
            _dialogueController.IsRandom = false;
            HomeRoomController.Instance.Bloom();
            arrow.SetActive(false);
            GameManager.Instance.gameFinished = true;
        }
        else if (!GameManager.Instance.HasSeenMother)
        {
            _dialogueController.SetDialogue(loreDialogue);
            _dialogueController.isPaused = false;
            _dialogueController.isIntermittent = false;
            _dialogueController.IsRandom = false;
        }
        else
        {
            _dialogueController.SetDialogue(randomDialogue);
            _dialogueController.isPaused = false;
            _dialogueController.isIntermittent = true;
            _dialogueController.IsRandom = true;
        }
        
        dialogueComponent.SetActive(true);
        _dialogueController.StartDialogue();
        _isTalkingToMother = true;
        _interactable.HidePromptText();
        
        if (GameManager.Instance.HasSeenMother == false)
        {
            GameManager.Instance.HasSeenMother = true;
        }
    }
    
    public void PanAwayFromMother()
    {
        // Set the camera to follow the player
        CameraController.Instance.panToMother = false;
            
        // Stop the dialogue
        // _dialogueController.StopDialogue();
        
        if (GameManager.Instance.gameFinished && _isTalkingToMother)
        {
            _interactable = GetComponentInChildren<Interactable>();
            _interactable.SetInteractable(false);
            StartCoroutine(LoadCredits());
        }

        _isTalkingToMother = false;
    }
    
    private IEnumerator LoadCredits()
    {
        // Save the game
        DataPersistenceManager.Instance.SaveGame();
        yield return new WaitForSeconds(1f);
        ScenesManager.LoadScene("Credits");
    }
}
