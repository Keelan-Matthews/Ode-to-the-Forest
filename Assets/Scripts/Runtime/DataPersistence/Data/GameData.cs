using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameData
{
    public int EssenceFragments; // The currency of the game
    public int Essence;
    public List<AbilityEffect> Abilities; // The abilities the player has equipped
    
    // Stores a single perma seed picked up in the dungeon
    public PermaSeed PermaSeed;
    // List to store the perma seeds
    public List<PermaSeed> ActivePermaSeeds;
    // How much essence is in the home base
    public int HomeEssence;
    
    public GameData()
    {
        EssenceFragments = 0;
        Essence = 0;
        Abilities = new List<AbilityEffect>();
        PermaSeed = null;
        ActivePermaSeeds = new List<PermaSeed>();
        HomeEssence = 0;
    }
}
