using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityIcon", menuName = "ScriptableObjects/UI/AbilityIcon")]
public class AbilityIcon : ScriptableObject
{
    public string abilityName;
    public Sprite abilitySprite;
}
