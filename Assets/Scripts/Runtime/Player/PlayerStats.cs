using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerStats : ScriptableObject
{
    public int essenceFragments; // The currency of the game
    public int essence;
    public int essenceQuantity = 5;
    public List<AbilityEffect> abilities = new (); // The abilities the player has equipped
}
