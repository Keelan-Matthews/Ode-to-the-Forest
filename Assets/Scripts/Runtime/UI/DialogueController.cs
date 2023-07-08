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
        if (textDisplay.text == _lines[_index] && !_isRandom)
        {
            NextLine();
        }
        else
        {
            StopAllCoroutines();
            textDisplay.text = _lines[_index];
        }
    }
    
    public void StopDialogue()
    {
        StopAllCoroutines();
        textDisplay.text = string.Empty;
        nameDisplay.text = string.Empty;
        
        // Disable the dialogue box
        gameObject.SetActive(false);
    }
    
    public void StartDialogue()
    {
        _index = 0;
        nameDisplay.text = dialogue.characterName;
        StartCoroutine(TypeLine());
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
        }
    }
    
    public void SetDialogue(Dialogue newDialogue)
    {
        this.dialogue = newDialogue;
    }
}
