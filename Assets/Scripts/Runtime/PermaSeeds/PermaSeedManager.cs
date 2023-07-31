using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermaSeedManager : MonoBehaviour, IDataPersistence
{
    public static PermaSeedManager Instance;
    [SerializeField] private List<PermaSeed> forestPermaSeeds = new();
    [SerializeField] private List<PermaSeed> _activePermaSeeds = new();
    // Stores a single perma seed picked up in the dungeon
    private PermaSeed _permaSeed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            DataPersistenceManager.Instance.RemoveDataPersistenceObject(gameObject.GetComponent<IDataPersistence>());
            Destroy(gameObject); // Destroy duplicate GameManager instances
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scene changes
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
    
    // this method gets the stored permaSeed
    public PermaSeed GetStoredPermaSeed()
    {
        return _permaSeed;
    }
    
    // this method sets the stored permaSeed
    public bool AddPermaSeed(PermaSeed seed)
    {
        // If the player already has a perma seed in their inventory, return false
        if (_permaSeed != null) return false;

        // Add the perma seed to the player's inventory
        _permaSeed = seed;
        // Update the inventory UI
        InventoryManager.Instance.AddPermaSeed(seed);

        return true;
    }
    
    public bool HasSeed()
    {
        // Check if the player has a perma seed
        return _permaSeed != null;
    }

    public bool HasSeed(PermaSeed seed)
    {
        // Check if the player has a specific perma seed
        return _permaSeed == seed;
    }

    public PermaSeed PlantSeed(int index)
    {
        // Get the player's perma seed
        var seed = _permaSeed;
        // Remove the perma seed from the player's inventory
        _permaSeed = null;
        // Update the inventory UI
        InventoryManager.Instance.RemovePermaSeed();

        return seed;
    }

    public void LoadData(GameData data)
    {
        _activePermaSeeds = data.ActivePermaSeeds;
        _permaSeed = data.PermaSeed;

        if (InventoryManager.Instance == null || _permaSeed == null) return;
        InventoryManager.Instance.AddPermaSeed(_permaSeed);
    }

    public void SaveData(GameData data)
    {
        data.ActivePermaSeeds = _activePermaSeeds;
        data.PermaSeed = _permaSeed;
    }
}