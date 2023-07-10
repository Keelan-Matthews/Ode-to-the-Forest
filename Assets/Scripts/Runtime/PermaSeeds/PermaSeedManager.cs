using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermaSeedManager : MonoBehaviour
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
        var permaSeed = floor switch
        {
            "Forest" => forestPermaSeeds[Random.Range(0, forestPermaSeeds.Count)],
            _ => null
        };
        
        while (_activePermaSeeds.Contains(permaSeed))
        {
            permaSeed = floor switch
            {
                "Forest" => forestPermaSeeds[Random.Range(0, forestPermaSeeds.Count)],
                _ => null
            };
        }
        
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
}