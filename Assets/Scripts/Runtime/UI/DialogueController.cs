using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DialogueController : MonoBehaviour
{
    public TextMeshProUGUI nameDisplay;
    public TextMeshProUGUI textDisplay;
    public GameObject triangleObject;
    private Image triangle;

    [Header("Dialogue Properties")]
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private Image characterImage;
    [SerializeField] private Sprite unknownCharacterSprite;
    [SerializeField] private bool unknownCharacter;
    private string[] _lines;
    public float textSpeed;
    private int _index;
    private bool _isRandom;
    public bool isIntermittent; // This means that every line is paused between
    public bool isPaused;

    private DialogueAudioInfo _currentAudioInfo;
    [SerializeField] private bool makePredictable = true;
    [SerializeField] private bool makeLower;
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
        _currentAudioInfo = dialogue.audioInfo;
        
        triangle = triangleObject.GetComponent<Image>();
        
        // Set the character image
        if (characterImage && !unknownCharacter)
        {
            characterImage.sprite = dialogue.characterImage;
        }
        else if (unknownCharacter)
        {
            characterImage.sprite = unknownCharacterSprite;
        }
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Space) && 
            !Input.GetKeyDown(KeyCode.Return) &&
            !Input.GetMouseButtonDown(0)
            // !Input.GetKeyDown(KeyCode.W) &&
            // !Input.GetKeyDown(KeyCode.UpArrow) && 
            // !Input.GetKeyDown(KeyCode.A) &&
            // !Input.GetKeyDown(KeyCode.LeftArrow) &&
            // !Input.GetKeyDown(KeyCode.S) &&
            // !Input.GetKeyDown(KeyCode.DownArrow) &&
            // !Input.GetKeyDown(KeyCode.D) &&
            // !Input.GetKeyDown(KeyCode.RightArrow)
            ) return;

        if (isPaused) return;

        if (textDisplay.maxVisibleCharacters == _lines[_index].Length)
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
            textDisplay.maxVisibleCharacters = _lines[_index].Length;
            triangle.enabled = true;
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
        isPaused = true;
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
        isPaused = false;

        // It will start the next line
        if (textDisplay.text == _lines[_index])
        {
            NextLine();
        }

        GameManager.Instance.activeDialogue = true;
    }

    public void StopDialogue()
    {
        // Only do so if the dialogue box is active
        // if (!gameObject && !gameObject.activeSelf) return;
        StopAllCoroutines();
        StartCoroutine(ExitDialogue());
        triangle.enabled = false;
    }
    
    private IEnumerator ExitDialogue()
    {
        yield return new WaitForSeconds(0.2f);
        Reset();
        gameObject.SetActive(false);
    }

    public void Reset()
    {
        textDisplay.text = string.Empty;
        nameDisplay.text = string.Empty;
        triangle.enabled = false;
        GameManager.Instance.activeDialogue = false;
    }

    public void StartDialogue()
    {
        triangle.enabled = false;
        _index = 0;
        nameDisplay.text = dialogue.characterName;
        
        // Set the character image
        if (characterImage && !unknownCharacter)
        {
            characterImage.sprite = dialogue.characterImage;
        }
        else if (unknownCharacter)
        {
            characterImage.sprite = unknownCharacterSprite;
        }
        
        _currentAudioInfo = dialogue.audioInfo;
        
        StartCoroutine(TypeLine());
        GameManager.Instance.activeDialogue = true;
        // AudioManager.PlaySound(AudioManager.Sound.OpenDialogue, transform.position);
    }

    private void PlayDialogueSound(int currentDisplayedCharacterCount, char currentCharacter, List<int> randomSoundIndexes)
    {
        var dialogueTypingSoundClips = _currentAudioInfo.dialogueTypingSoundClips;
        var minFrequencyLevel = _currentAudioInfo.minFrequencyLevel;
        var maxFrequencyLevel = _currentAudioInfo.maxFrequencyLevel;
        var minPitch = _currentAudioInfo.minPitch;
        var maxPitch = _currentAudioInfo.maxPitch;
        var stopAudioSource = _currentAudioInfo.stopAudioSource;
        
        var frequency = Random.Range(minFrequencyLevel, maxFrequencyLevel);
        
        if (currentDisplayedCharacterCount % frequency != 0 || randomSoundIndexes.Count == 0) return;

        if (stopAudioSource)
        {
            _audioSource.Stop();
        }
        
        // Create predictable audio from hashing
        AudioClip soundClip;
        if (makePredictable)
        {
            var hash = currentCharacter.GetHashCode();
            // var predictableIndex = hash % dialogueTypingSoundClips.Length;
            // soundClip = dialogueTypingSoundClips[predictableIndex];
            
            // Make the soundClip equal to a randomSoundIndex, then remove it from the list
            var randomIndex = Random.Range(0, randomSoundIndexes.Count);
            var predictableIndex = randomSoundIndexes[randomIndex];
            soundClip = dialogueTypingSoundClips[predictableIndex];
            randomSoundIndexes.RemoveAt(randomIndex);
            
            // If makeLower is true, make min and max pitch lower
            if (makeLower)
            {
                minPitch -= 0.16f;
                maxPitch -= 0.16f;
            }

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
        // Lower the volume
        _audioSource.volume = 0.8f;
        _audioSource.PlayOneShot(soundClip);
    }

    private IEnumerator TypeLine()
    {
        triangle.enabled = false;
        // If is random is true, get a single random line
        if (_isRandom)
        {
            _index = Random.Range(0, _lines.Length);
        }
        
        var dialogueTypingSoundClips = _currentAudioInfo.dialogueTypingSoundClips;
        var oneSound = _currentAudioInfo.oneSound;
        // Pick 3 to 5 random sounds from dialogueTypingSoundClips, ensuring that there are no duplicates,
        // if oneSound is true, only pick one sound
        int randomSoundCount;
        if (oneSound)
        {
            randomSoundCount = 1;
        }
        else
        {
            randomSoundCount = Random.Range(3, 5);
        }
        var randomSoundIndexes = new List<int>();
        for (var i = 0; i < randomSoundCount; i++)
        {
            var randomIndex = Random.Range(0, dialogueTypingSoundClips.Length);
            while (randomSoundIndexes.Contains(randomIndex))
            {
                randomIndex = Random.Range(0, dialogueTypingSoundClips.Length);
            }
            randomSoundIndexes.Add(randomIndex);
        }

        textDisplay.text = _lines[_index];
        textDisplay.maxVisibleCharacters = 0;
        
        foreach (var letter in _lines[_index].ToCharArray())
        {
            PlayDialogueSound(textDisplay.maxVisibleCharacters, letter, randomSoundIndexes);

            textDisplay.maxVisibleCharacters++;
            yield return new WaitForSeconds(textSpeed);
        }

        triangle.enabled = true;
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