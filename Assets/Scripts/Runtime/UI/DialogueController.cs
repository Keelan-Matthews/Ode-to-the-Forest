using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public TextMeshProUGUI nameDisplay;
    public TextMeshProUGUI textDisplay;

    public Dialogue dialogue;
    private string[] _lines;
    public float textSpeed;
    private int _index;
    private bool _isRandom;
    public bool isIntermittent; // This means that every line is paused between
    private bool _isPaused;

    public bool IsRandom
    {
        get => _isRandom;
        set => _isRandom = value;
    }

    private void Awake()
    {
        nameDisplay.text = string.Empty;
        textDisplay.text = string.Empty;

        // Set the lines
        _lines = dialogue.sentences;
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Space) && !Input.GetKeyDown(KeyCode.Return) &&
            !Input.GetMouseButtonDown(0)) return;

        if (_isPaused) return;

        if (textDisplay.text == _lines[_index] && !_isRandom)
        {
            if (isIntermittent)
            {
                // If it is intermittent, interacting will pause the dialogue
                PauseDialogue();
            }
            else
            {
                NextLine();
            }
        }
        else
        {
            StopAllCoroutines();
            textDisplay.text = _lines[_index];
        }
    }

    public void PauseDialogue()
    {
        // This will hide the dialogue box and retain where the player is in the dialogue
        gameObject.SetActive(false);
        _isPaused = true;
        GameManager.Instance.activeDialogue = false;
        
        // if the active room was meant to have a wave, start it
        if (GameManager.Instance.activeRoom.hasWave)
        {
            GameManager.Instance.activeRoom.hasDialogue = false;
            // Trigger RoomController OnRoomChange event again
            RoomController.Instance.InvokeOnRoomChange();
        }
    }

    public void ResumeDialogue()
    {
        // This will show the dialogue box and continue where the player left off
        gameObject.SetActive(true);
        _isPaused = false;

        // It will start the next line
        if (textDisplay.text == _lines[_index] && !_isRandom)
        {
            NextLine();
        }
        
        GameManager.Instance.activeDialogue = true;
    }

    public void StopDialogue()
    {
        StopAllCoroutines();
        textDisplay.text = string.Empty;
        nameDisplay.text = string.Empty;

        // Disable the dialogue box
        gameObject.SetActive(false);
        GameManager.Instance.activeDialogue = false;
    }

    public void StartDialogue()
    {
        _index = 0;
        nameDisplay.text = dialogue.characterName;
        StartCoroutine(TypeLine());
        GameManager.Instance.activeDialogue = true;
    }

    private IEnumerator TypeLine()
    {
        // If is random is true, get a single random line
        if (_isRandom)
        {
            _index = Random.Range(0, _lines.Length);
        }

        foreach (var letter in _lines[_index].ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    private void NextLine()
    {
        if (_index < _lines.Length - 1)
        {
            _index++;

            textDisplay.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            textDisplay.text = string.Empty;
            nameDisplay.text = string.Empty;

            // Disable the dialogue box
            gameObject.SetActive(false);
            GameManager.Instance.activeDialogue = false;
        }
    }

    public void SetDialogue(Dialogue newDialogue)
    {
        this.dialogue = newDialogue;
    }
}