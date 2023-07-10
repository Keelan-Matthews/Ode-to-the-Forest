using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject dialogueBox;
    public DialogueController dialogueController;

    public static TutorialManager Instance;
    
    private void Awake()
    {
        Instance = this;
    }
    
    // Start is called before the first frame update
    private void Start()
    {
        PlayerController.Instance.SetInvincible(true);
        dialogueController = dialogueBox.GetComponent<DialogueController>();
    }
    
    public void StartTutorial()
    {
        dialogueBox.SetActive(true);
        dialogueController.StartDialogue();
    }
    
    public void ResumeTutorial()
    {
        dialogueBox.SetActive(true);
        dialogueController.ResumeDialogue();
    }
}
