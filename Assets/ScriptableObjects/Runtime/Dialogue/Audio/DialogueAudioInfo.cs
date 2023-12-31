using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueAudioInfo", menuName = "ScriptableObjects/DialogueAudioInfo", order = 1)]
public class DialogueAudioInfo : ScriptableObject
{
    public string id;
    public AudioClip[] dialogueTypingSoundClips;
    [Range(1, 40)]
    public int minFrequencyLevel = 2;
    [Range(1, 40)]
    public int maxFrequencyLevel = 5;
    [Range(-3, 3)]
    public float minPitch = 0.5f;
    [Range(-3, 3)]
    public float maxPitch = 3f;
    public bool stopAudioSource;
    public bool oneSound;
}
