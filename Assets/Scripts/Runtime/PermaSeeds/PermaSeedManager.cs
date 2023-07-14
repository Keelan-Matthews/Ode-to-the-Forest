using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermaSeedManager : MonoBehaviour, IDataPersistence
{
    public static PermaSeedManager Instance;
    [SerializeField] private List<PermaSeed> forestPermaSeeds = new();
    [SerializeField] private List<PermaSeed> _activePermaSeeds = new();

    private void Awake()
    {
        Instance = this;
    }

    public PermaSeed GetRandomPermaSeed()
    {
        // Get the current floor from the GameManager
        var floor = GameManager.Instance.currentWorldName;

        // Get a random perma seed from the list of perma seeds for the current floor
        // if the player has that seed active already, get a different one
        PermaSeed permaSeed;
        do
        {
            permaSeed = floor switch
            {
                "Forest" => forestPermaSeeds[Random.Range(0, forestPermaSeeds.Count)],
                _ => null
            };
        } while (_activePermaSeeds.Contains(permaSeed));

        return permaSeed;
    }
    
    public PermaSeed GetSpecificPermaSeed(string permaSeedName)
    {
        // Get the current floor from the GameManager
        var floor = GameManager.Instance.currentWorldName;

        // Get a random perma seed from the list of perma seeds for the current floor
        // if the player has that seed active already, get a different one
        var permaSeed = floor switch
        {
            "Forest" => forestPermaSeeds.Find(seed => seed.name == permaSeedName),
            _ => null
        };
        
        return permaSeed;
    }
    
    public List<PermaSeed> GetActiveSeeds()
    {
        return _activePermaSeeds;
    }

    public void AddActiveSeed(PermaSeed seed)
    {
        // Add a perma seed to the player's active seeds
        _activePermaSeeds.Add(seed);
    }

    public void UprootSeed(PermaSeed seed)
    {
        // Remove a perma seed from the player's active seeds
        _activePermaSeeds.Remove(seed);
    }

    // This function calls remove on all the active seeds
    public void RemoveActiveSeeds()
    {
        foreach (var seed in _activePermaSeeds)
        {
            seed.Remove();
        }
    }

    public void LoadData(GameData data)
    {
        // Load any active perma seeds
        _activePermaSeeds = data.ActivePermaSeeds;
    }

    public void SaveData(GameData data)
    {
        // Save any active perma seeds
        data.ActivePermaSeeds = _activePermaSeeds;
    }
}