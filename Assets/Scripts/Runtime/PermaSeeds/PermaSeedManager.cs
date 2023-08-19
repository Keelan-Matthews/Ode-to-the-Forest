using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class PermaSeedManager : MonoBehaviour, IDataPersistence
{
    public static PermaSeedManager Instance;
    [SerializeField] private List<PermaSeed> permanentPermaSeeds = new();
    [SerializeField] private List<PermaSeed> commonPermaSeeds = new();
    [SerializeField] private List<PermaSeed> rarePermaSeeds = new();
    [SerializeField] private List<PermaSeed> legendaryPermaSeeds = new();
    [SerializeField] private List<PermaSeed> activePermaSeeds = new();
    
    [Header("Essence per seed rarity")]
    [SerializeField] private int commonEssenceAmount;
    [SerializeField] private int rareEssenceAmount;
    [SerializeField] private int legendaryEssenceAmount;
    // Stores a single perma seed picked up in the dungeon
    private PermaSeed _permaSeed;
    
    [Header("Data Persistence")]
    [SerializeField] private bool firstLoad;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate GameManager instances
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scene changes
        
        SetPermaSeedAmounts();
    }
    
    private void SetPermaSeedAmounts()
    {
        // Set the essence amounts for each seed rarity
        foreach (var seed in permanentPermaSeeds)
        {
            seed.essenceRequired = 0;
        }
        foreach (var seed in commonPermaSeeds)
        {
            seed.essenceRequired = commonEssenceAmount;
        }
        foreach (var seed in rarePermaSeeds)
        {
            seed.essenceRequired = rareEssenceAmount;
        }
        foreach (var seed in legendaryPermaSeeds)
        {
            seed.essenceRequired = legendaryEssenceAmount;
        }
    }

    public PermaSeed GetRandomPermaSeed(int difficulty)
    {
        // Get the current floor from the GameManager
        var floor = GameManager.Instance.currentWorldName;

        PermaSeed permaSeed;
        
        // Choose a random perma seed based on the difficulty (0,1,2),
        // and keep choosing while the player already has that seed active
        do
        {
            permaSeed = floor switch
            {
                "Forest" => difficulty switch
                {
                    0 => commonPermaSeeds[Random.Range(0, commonPermaSeeds.Count)],
                    1 => rarePermaSeeds[Random.Range(0, rarePermaSeeds.Count)],
                    2 => legendaryPermaSeeds[Random.Range(0, legendaryPermaSeeds.Count)],
                    _ => null
                },
                _ => null
            };
        } while (activePermaSeeds.Contains(permaSeed) || _permaSeed == permaSeed);

        return permaSeed;
    }
    
    public bool HasAllSeeds()
    {
        // Check if the player has all the perma seeds
        return activePermaSeeds.Count == commonPermaSeeds.Count + rarePermaSeeds.Count + legendaryPermaSeeds.Count;
    }
    
    public PermaSeed GetSpecificPermaSeed(string permaSeedName)
    {
        // Get the current floor from the GameManager
        var floor = GameManager.Instance.currentWorldName;

        // Get a random perma seed from the list of perma seeds for the current floor
        // if the player has that seed active already, get a different one
        var permaSeed = floor switch
        {
            "Forest" => commonPermaSeeds.Find(seed => seed.seedName == permaSeedName),
            _ => null
        };
        
        if (permaSeed == null)
        {
            permaSeed = floor switch
            {
                "Forest" => permanentPermaSeeds.Find(seed => seed.seedName == permaSeedName),
                _ => null
            };
        }

        if (permaSeed == null)
        {
            permaSeed = floor switch
            {
                "Forest" => rarePermaSeeds.Find(seed => seed.seedName == permaSeedName),
                _ => null
            };
        }
        
        if (permaSeed == null)
        {
            permaSeed = floor switch
            {
                "Forest" => legendaryPermaSeeds.Find(seed => seed.seedName == permaSeedName),
                _ => null
            };
        }
        
        return permaSeed;
    }
    
    public void RemoveDuplicateActiveSeeds()
    {
        // Remove any duplicate perma seeds from the player's active seeds
        activePermaSeeds = activePermaSeeds.Distinct().ToList();
    }
    
    public List<PermaSeed> GetActiveSeeds()
    {
        RemoveDuplicateActiveSeeds();
        return activePermaSeeds;
    }

    public void AddActiveSeed(PermaSeed seed)
    {
        // Add a perma seed to the player's active seeds
        activePermaSeeds.Add(seed);
    }

    public void UprootSeed(PermaSeed seed)
    {
        // Remove a perma seed from the player's active seeds
        activePermaSeeds.Remove(seed);
    }

    // This function calls remove on all the active seeds
    public void RemoveActiveSeeds()
    {
        foreach (var seed in activePermaSeeds)
        {
            seed.Remove();
        }
    }
    
    // this method gets the stored permaSeed
    public PermaSeed GetStoredPermaSeed()
    {
        return _permaSeed;
    }
    
    public void RemoveStoredPermaSeed()
    {
        // Remove the perma seed from the player's inventory
        _permaSeed = null;
        // Update the inventory UI
        InventoryManager.Instance.RemovePermaSeed();
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
        if (data.ActivePermaSeeds.Count > 0)
        {
            // Get the specific perma seed for each seed name in the list of active seeds
            foreach (var seedName in data.ActivePermaSeeds)
            {
                var seed = GetSpecificPermaSeed(seedName);
                if (seed == null) continue;
                // Add the perma seed to the player's active seeds if one with the same name isn't already active
                if (!activePermaSeeds.Contains(seed))
                {
                    activePermaSeeds.Add(seed);
                }
            }
        }

        if (data.PermaSeed.Length > 0)
        {
            // Get the specific perma seed for the seed name in the data
            var seed = GetSpecificPermaSeed(data.PermaSeed);
            if (seed == null) return;
            // Add the perma seed to the player's inventory
            _permaSeed = seed;
        }

        if (InventoryManager.Instance == null || _permaSeed == null) return;
        InventoryManager.Instance.AddPermaSeed(_permaSeed);
    }

    public void SaveData(GameData data)
    {
        // Add the name of each active perma seed to the list of active seeds in the data
        foreach (var seed in activePermaSeeds)
        {
            // If the seed is already in the list, skip it
            if (data.ActivePermaSeeds.Contains(seed.seedName)) continue;
            data.ActivePermaSeeds.Add(seed.seedName);
        }

        // Add the name of the perma seed in the player's inventory to the data
        if (_permaSeed != null)
        {
            data.PermaSeed = _permaSeed.seedName;
        }
        else
        {
            data.PermaSeed = "";
        }
    }

    public bool FirstLoad()
    {
        return firstLoad;
    }
}