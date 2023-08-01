using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameData
{
    public long LastUpdated;
    public Vector3 PlayerPosition;
    public int EssenceFragments; // The currency of the game
    public int Essence;
    public List<AbilityEffect> Abilities; // The abilities the player has equipped
    
    // Stores a single perma seed picked up in the dungeon
    public PermaSeed PermaSeed;
    // List to store the perma seeds
    public List<PermaSeed> ActivePermaSeeds;
    // List that stores which seed plots have what seed
    public PermaSeed[] SeedPlotSeeds;
    // Stores which seeds are grown
    public bool[] GrownSeeds;
    // Stores which plots have been unlocked
    public bool[] UnlockedPlots;
    // How much essence is in the home base
    public int HomeEssence;
    // Active scene
    public string CurrentSceneName;
    
    // Is tutorial
    public bool IsTutorial;
    
    // Settings variables
    public float MasterVolume;
    public float MusicVolume;
    public float SfxVolume;
    
    public GameData()
    {
        PlayerPosition = Vector3.zero;
        EssenceFragments = 0;
        Essence = 0;
        Abilities = new List<AbilityEffect>();
        PermaSeed = null;
        ActivePermaSeeds = new List<PermaSeed>();
        HomeEssence = 0;
        CurrentSceneName = "";
        IsTutorial = true;
        SeedPlotSeeds = new PermaSeed[4];
        GrownSeeds = new bool[4];
        UnlockedPlots = new bool[4];
        
        // Settings
        MasterVolume = 0.6f;
        MusicVolume = 0.6f;
        SfxVolume = 0.6f;
    }
}
