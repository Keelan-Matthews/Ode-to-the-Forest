using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public TextMeshProUGUI nameDisplay;
    public TextMeshProUGUI textDisplay;

    [Header("Dialogue Properties")]
    [SerializeField] private Dialogue dialogue;
    private string[] _lines;
    public float textSpeed;
    private int _index;
    private bool _isRandom;
    public bool isIntermittent; // This means that every line is paused between
    private bool _isPaused;

    [Header("Audio")] 
    [SerializeField] private DialogueAudioInfo defaultAudioInfo;
    private DialogueAudioInfo _currentAudioInfo;
    [SerializeField] private bool makePredictable = true;
    private AudioSource _audioSource;

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
        if (dialogue)
        {
            _lines = dialogue.sentences;
        }
        
        // Create the audio source
        _audioSource = gameObject.AddComponent<AudioSource>();
        _currentAudioInfo = defaultAudioInfo;
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
        StartCoroutine(PauseDialogueDelay());
    }

    private IEnumerator PauseDialogueDelay()
    {
        yield return new WaitForSeconds(0.2f);
        
        // This will hide the dialogue box and retain where the player is in the dialogue
        gameObject.SetActive(false);
        _isPaused = true;
        GameManager.Instance.activeDialogue = false;
        
        // if the active room was meant to have a wave, start it
        if (GameManager.Instance.activeRoom && GameManager.Instance.activeRoom.hasWave)
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
        // Only do so if the dialogue box is active
        if (!gameObject.activeSelf) return;
        StopAllCoroutines();
        StartCoroutine(ExitDialogue());
    }
    
    private IEnumerator ExitDialogue()
    {
        yield return new WaitForSeconds(0.2f);
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

    private void PlayDialogueSound(int currentDisplayedCharacterCount, char currentCharacter)
    {
        var dialogueTypingSoundClips = _currentAudioInfo.dialogueTypingSoundClips;
        var frequencyLevel = _currentAudioInfo.frequencyLevel;
        var minPitch = _currentAudioInfo.minPitch;
        var maxPitch = _currentAudioInfo.maxPitch;
        var stopAudioSource = _currentAudioInfo.stopAudioSource;

        if (currentDisplayedCharacterCount % frequencyLevel != 0) return;
        if (stopAudioSource)
        {
            _audioSource.Stop();
        }
        
        // Create predictable audio from hashing
        AudioClip soundClip;
        if (makePredictable)
        {
            var hash = currentCharacter.GetHashCode();
            var predictableIndex = hash % dialogueTypingSoundClips.Length;
            soundClip = dialogueTypingSoundClips[predictableIndex];
            
            // Pitch
            var minPitchInt = (int)(minPitch * 100);
            var maxPitchInt = (int)(maxPitch * 100);
            var pitchRangeInt = maxPitchInt - minPitchInt;
            
            if (pitchRangeInt == 0)
            {
                _audioSource.pitch = minPitch;
            }
            else
            {
                var predictablePitchInt = (hash % pitchRangeInt) + minPitchInt;
                var predictablePitch = predictablePitchInt / 100f;
                _audioSource.pitch = predictablePitch;
            }
        }
        else
        {
            var randomIndex = Random.Range(0, dialogueTypingSoundClips.Length);
            soundClip = dialogueTypingSoundClips[randomIndex];
            _audioSource.pitch = Random.Range(minPitch, maxPitch);
        }
        
        // Set the mixer group
        _audioSource.outputAudioMixerGroup = GameAssets.Instance.AudioMixer.FindMatchingGroups("SFX").First();
        _audioSource.PlayOneShot(soundClip);
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
            PlayDialogueSound(textDisplay.text.Length, letter);
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
            StartCoroutine(ExitDialogue());
        }
    }

    public void SetDialogue(Dialogue newDialogue)
    {
        dialogue = newDialogue;
        _lines = dialogue.sentences;
    }
}