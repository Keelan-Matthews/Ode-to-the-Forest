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
        var maxIterations = 100;
        var iterations = 0;

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
            iterations++;
        } while (iterations < maxIterations && (activePermaSeeds.Contains(permaSeed) || _permaSeed == permaSeed));

        return permaSeed;
    }
    
    public bool HasAllSeeds()
    {
        // Check if the player has all the perma seeds
        return activePermaSeeds.Count == commonPermaSeeds.Count + rarePermaSeeds.Count + legendaryPermaSeeds.Count;
    }

    public bool HasAllSeeds(string difficulty)
    {

        return difficulty switch
        {
            "Easy" => CountNumberOfSeeds("common") == commonPermaSeeds.Count,
            "Medium" => CountNumberOfSeeds("rare") == rarePermaSeeds.Count,
            "Hard" => CountNumberOfSeeds("legendary") == legendaryPermaSeeds.Count,
            _ => false
        };
    }

    private int CountNumberOfSeeds(string rarity)
    {
        var total = 0;
        // If rarity = common, see how many of the common seeds the player has active or in their inventory
        // If rarity = rare, see how many of the rare seeds the player has active or in their inventory
        // If rarity = legendary, see how many of the legendary seeds the player has active or in their inventory
        total = rarity switch
        {
            "common" => activePermaSeeds.Count(seed => commonPermaSeeds.Contains(seed)),
            "rare" => activePermaSeeds.Count(seed => rarePermaSeeds.Contains(seed)),
            "legendary" => activePermaSeeds.Count(seed => legendaryPermaSeeds.Contains(seed)),
            _ => 0
        };
        
        // Now see what rarity the perma seed is
        if (_permaSeed == null) return total;
        var permaSeedRarity = GetPermaSeedRarity(_permaSeed.seedName);
        
        if (permaSeedRarity == rarity)
        {
            total++;
        }
        
        return total;
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
    
    public void UnapplyPermaSeedEffects()
    {
        // Unapply the effects of all the perma seeds the player has active
        foreach (var seed in activePermaSeeds)
        {
            seed.GetAbilityEffect().Unapply(PlayerController.Instance.gameObject);
        }
        
        // Clear the list of active perma seeds
        activePermaSeeds.Clear();
    }
    
    public string GetPermaSeedRarity(string permaSeedName)
    {
        // Get the current floor from the GameManager
        var floor = GameManager.Instance.currentWorldName;

        var rarity = "common";

        // Get a random perma seed from the list of perma seeds for the current floor
        // if the player has that seed active already, get a different one
        var permaSeed = floor switch
        {
            "Forest" => commonPermaSeeds.Find(seed => seed.seedName == permaSeedName),
            _ => null
        };
        
        if (permaSeed == null)
        {
            rarity = "permanent";
            
            permaSeed = floor switch
            {
                "Forest" => permanentPermaSeeds.Find(seed => seed.seedName == permaSeedName),
                _ => null
            };
        }

        if (permaSeed == null)
        {
            rarity = "rare";
            
            permaSeed = floor switch
            {
                "Forest" => rarePermaSeeds.Find(seed => seed.seedName == permaSeedName),
                _ => null
            };
        }
        
        if (permaSeed == null)
        {
            rarity = "legendary";
            
            permaSeed = floor switch
            {
                "Forest" => legendaryPermaSeeds.Find(seed => seed.seedName == permaSeedName),
                _ => null
            };
        }

        return rarity;
    }

    public List<PermaSeed> GetActiveSeeds()
    {
        // RemoveDuplicateActiveSeeds();
        return activePermaSeeds;
    }

    public void AddActiveSeed(PermaSeed seed)
    {
        // Add a perma seed to the player's active seeds
        if (!activePermaSeeds.Contains(seed))
        {
            activePermaSeeds.Add(seed);
        }
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
    public void AddPermaSeed(PermaSeed seed)
    {
        // If the player already has a perma seed in their inventory, return false
        // if (_permaSeed != null) return false;

        // Add the perma seed to the player's inventory
        _permaSeed = seed;

        if (InventoryManager.Instance == null || _permaSeed == null) return;
        // Update the inventory UI
        InventoryManager.Instance.AddPermaSeed(seed);
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
    
    public bool HasSeed(string seedName)
    {
        // Check if the player has a specific perma seed
        return _permaSeed != null && _permaSeed.seedName == seedName;
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
        if (data.PermaSeed.Length > 0)
        {
            // Get the specific perma seed for the seed name in the data
            var seed = GetSpecificPermaSeed(data.PermaSeed);
            if (seed == null) return;
            AddPermaSeed(seed);
        }
    }

    public void SaveData(GameData data)
    {
        if (ScenesManager.Instance.currentSceneName == "ForestMain" &&
            PlayerController.Instance.GetComponent<Health>().HealthValue > 0) return;
        data.ActivePermaSeeds.Clear();
        // Add the name of each active perma seed to the list of active seeds in the data
        foreach (var seed in activePermaSeeds)
        {
            // Clear the list of active perma seeds in the data
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
    
    public bool IsActive()
    {
        return gameObject.activeSelf;
    }
}