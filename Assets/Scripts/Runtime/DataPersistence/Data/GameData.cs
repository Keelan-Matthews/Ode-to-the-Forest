using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameData
{
    public long LastUpdated;
    public int EssenceFragments; // The currency of the game
    public int Essence;
    public List<string> Abilities; // The abilities the player has equipped
    public List<string> PurchasedAbilities; // The abilities the player has purchased
    
    public string PermaSeed;
    public List<string> ActivePermaSeeds;
    public string[] SeedPlotSeeds;
    public bool[] GrownSeeds;
    public bool[] UnlockedPlots;
    public int HomeEssence;
    public string CurrentSceneName;
    // Day
    public int Day;
    public bool gameFinished;
    public bool deeperPortalSpawn;
    public bool deeperPortalSpawnPrompted;
    
    public bool IsTutorial;
    public bool HasSeenTrader;
    public bool HasSeenCollector;
    public bool HasSeenMinimapTutorial;
    public int TimesEnteredDungeon;
    
    // Shrine of Youth
    public bool[] fountainActivated;
    
    public GameData()
    {
        EssenceFragments = 0;
        Essence = 0;
        Day = 0;
        TimesEnteredDungeon = 0;
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
    }
}
