using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OracleDialogueController : MonoBehaviour
{
    [SerializeField] private Dialogue oracleDialogue;
    
    [SerializeField] private Dialogue dialogueClairvoyance;
    [SerializeField] private Dialogue dialogueGoodLuck;
    [SerializeField] private Dialogue dialogueMarker;

    [SerializeField] private GameObject dialogueComponent;
    private DialogueController _dialogueController;
    
    [SerializeField] private ParticleSystem clairvoyanceParticles;
    [SerializeField] private ParticleSystem goodLuckParticles;
    [SerializeField] private ParticleSystem markerParticles;

    private bool _isTalkingToOracle;

    private void Awake()
    {
        _dialogueController = dialogueComponent.GetComponent<DialogueController>();
    }

    private void Update()
    {
        _isTalkingToOracle = GameManager.Instance.activeDialogue;
    }

    public void TalkToOracle()
    {
        if (_isTalkingToOracle) return;
        _dialogueController.isPaused = false;
        
        _dialogueController.isIntermittent = true;
        _dialogueController.IsRandom = true;
        _dialogueController.SetDialogue(oracleDialogue);
        dialogueComponent.SetActive(true);

        if (_dialogueController.isPaused)
        {
            _dialogueController.ResumeDialogue();
            
        }
        else
        {
            _dialogueController.StartDialogue();
        }
        
        _isTalkingToOracle = true;
        var interactable = GetComponentInChildren<Interactable>();
        interactable.HidePromptText();
    }
    
    public void StopTalkingToOracle()
    {
        // _dialogueController.StopDialogue();
        _isTalkingToOracle = false;
    }
    
    public void Clairvoyance()
    {
        if (_isTalkingToOracle) return;
        clairvoyanceParticles.Play();
        AudioManager.PlaySound(AudioManager.Sound.SeedGrown, transform.position);
        _dialogueController.SetDialogue(dialogueClairvoyance);
        dialogueComponent.SetActive(true);
        _dialogueController.StartDialogue();
        _isTalkingToOracle = true;
    }
    
    public void GoodLuck()
    {
        if (_isTalkingToOracle) return;
        goodLuckParticles.Play();
        AudioManager.PlaySound(AudioManager.Sound.SeedGrown, transform.position);
        _dialogueController.SetDialogue(dialogueGoodLuck);
        dialogueComponent.SetActive(true);
        _dialogueController.StartDialogue();
        _isTalkingToOracle = true;
        
    }
    
    public void Marker()
    {
        if (_isTalkingToOracle) return;
        markerParticles.Play();
        AudioManager.PlaySound(AudioManager.Sound.SeedGrown, transform.position);
        _dialogueController.SetDialogue(dialogueMarker);
        dialogueComponent.SetActive(true);
        _dialogueController.StartDialogue();
        _isTalkingToOracle = true;
    }
}
