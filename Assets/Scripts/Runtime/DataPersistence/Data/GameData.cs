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
    public List<string> Abilities; // The abilities the player has equipped
    public List<string> PurchasedAbilities; // The abilities the player has purchased
    
    // Stores a single perma seed picked up in the dungeon
    public string PermaSeed;
    // List to store the perma seeds
    public List<string> ActivePermaSeeds;
    // List that stores which seed plots have what seed
    public string[] SeedPlotSeeds;
    // Stores which seeds are grown
    public bool[] GrownSeeds;
    // Stores which plots have been unlocked
    public bool[] UnlockedPlots;
    // How much essence is in the home base
    public int HomeEssence;
    // Active scene
    public string CurrentSceneName;
    // Day
    public int Day;
    public bool gameFinished;
    public bool deeperPortalSpawn;
    public bool deeperPortalSpawnPrompted;
    
    // Is tutorial
    public bool IsTutorial;
    public bool HasSeenTrader;
    public bool HasSeenCollector;
    
    // Settings variables
    public float MasterVolume;
    public float MusicVolume;
    public float SfxVolume;
    
    // Shrine of Youth
    public bool[] fountainActivated;
    
    public GameData()
    {
        PlayerPosition = Vector3.zero;
        EssenceFragments = 0;
        Essence = 0;
        Day = 0;
        Abilities = new List<string>();
        PurchasedAbilities = new List<string>();
        PermaSeed = null;
        ActivePermaSeeds = new List<string>();
        HomeEssence = 0;
        CurrentSceneName = "";
        IsTutorial = true;
        SeedPlotSeeds = new string[4];
        GrownSeeds = new bool[4];
        UnlockedPlots = new bool[4];
        fountainActivated = new bool[4];
        
        // Initialize fountain activated to all false
        for (var i = 0; i < fountainActivated.Length; i++)
        {
            fountainActivated[i] = false;
        }
        
        // Settings
        MasterVolume = 0.7f;
        MusicVolume = 0.7f;
        SfxVolume = 0.7f;
    }
}
