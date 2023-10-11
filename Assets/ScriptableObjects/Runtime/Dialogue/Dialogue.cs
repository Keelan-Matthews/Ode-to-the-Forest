using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{
    public string characterName;
    [TextArea(3, 10)]
    public string[] sentences;
    public Sprite characterImage;
    public DialogueAudioInfo audioInfo;
}
