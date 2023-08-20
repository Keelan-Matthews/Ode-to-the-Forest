using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Runtime.Abilities
{
    public class AbilityManager : MonoBehaviour, IDataPersistence
    {
        // This is a singleton that manages the abilities.
        // It contains pools of abilities that pertain to the floor the player is on.
        // It also contains a list of all abilities.
        // It is responsible for giving the player a random ability.

        public static AbilityManager Instance;
        public GameObject abilityInformation;
        private bool _rerolled;

        [SerializeField] private List<AbilityEffect> forestAbilities = new();
        [SerializeField] private List<AbilityEffect> _purchasedAbilities = new();
        
        public static event Action<AbilityEffect> OnAbilityPurchased;
        
        private void Awake()
        {
            Instance = this;
        }
        
        public AbilityEffect GetRandomAbility()
        {
            // Get the current floor from the GameManager
            var floor = GameManager.Instance.currentWorldName;
            
            // Get a random ability from the list of abilities for the current floor
            // keep trying until we get an ability that the player doesn't already have
            var permaSeeds = PermaSeedManager.Instance.GetActiveSeeds();
            // get the abilities from the perma seeds
            var permaSeedAbilities = new List<AbilityEffect>();
            foreach (var permaSeed in permaSeeds)
            {
                permaSeedAbilities.Add(permaSeed.abilityEffect);
            }
            
            AbilityEffect ability;
            do
            {
                ability = floor switch
                {
                    "Forest" => forestAbilities[Random.Range(0, forestAbilities.Count)],
                    _ => null
                };
            } while (permaSeedAbilities.Contains(ability));
            
            // If Clover seed is active and the ability is a downgrade, reroll
            // if (permaSeedAbilities.Contains(PermaSeedManager.Instance.GetSpecificPermaSeed("Clover").abilityEffect) && !ability.IsUpgrade() && !_rerolled)
            // {
            //     _rerolled = true;
            //     return GetRandomAbility();
            // }
            
            // Return the ability
            return ability;
        }

        public AbilityEffect GetObeliskAbility()
        {
            var ability = GetRandomAbility();
            OnAbilityPurchased?.Invoke(ability);
            return ability;
        }
        
        public void PurchaseAbility(AbilityEffect abilityEffect)
        {
            // Add the ability to the list of purchased abilities
            _purchasedAbilities.Add(abilityEffect);
            
            // Invoke the OnAbilityPurchased event
            OnAbilityPurchased?.Invoke(abilityEffect);
        }
        
        public List<AbilityEffect> GetPurchasedAbilities()
        {
            return _purchasedAbilities;
        }

        public AbilityEffect GetAbility(string abilityName)
        {
            // Get the current floor from the GameManager
            var floor = GameManager.Instance.currentWorldName;
            
            // Get the ability from the list of abilities for the current floor
            return floor switch
            {
                "Forest" => forestAbilities.Find(ability => ability.name == abilityName),
                _ => null
            };
        }
        
        public List<AbilityEffect> GetAbilities()
        {
            // Get the current floor from the GameManager
            var floor = GameManager.Instance.currentWorldName;
            
            // Get the list of abilities for the current floor
            return floor switch
            {
                "Forest" => forestAbilities,
                _ => null
            };
        }
        
        // Removes an ability
        public void RemoveAbility(string abilityName)
        {
            // Get the current floor from the GameManager
            var floor = GameManager.Instance.currentWorldName;
            
            // Remove the ability from the list of abilities for the current floor
            switch (floor)
            {
                case "Forest":
                    forestAbilities.Remove(forestAbilities.Find(ability => ability.name == abilityName));
                    break;
            }
        }
        
        // This function takes in an ability and displays its stats in the UI
        public void DisplayAbilityStats(AbilityEffect abilityEffect)
        {
            abilityInformation.SetActive(true);
            
            // Get the child "AbilityName" text object
            var abilityName = abilityInformation.transform.Find("AbilityName").GetComponent<TextMeshProUGUI>();
            // Set the text to the ability's name
            abilityName.text = abilityEffect.abilityName;
            
            var abilityDescription = abilityInformation.transform.Find("AbilityDescription").GetComponent<TextMeshProUGUI>();
            // Set the text to the ability's description
            abilityDescription.text = abilityEffect.description;
            
            // SetActive to false after 2 seconds
            Invoke(nameof(DisableAbilityInformation), 4f);
        }
        
        private void DisableAbilityInformation()
        {
            abilityInformation.SetActive(false);
        }

        public void LoadData(GameData data)
        {
            // Load each purchased ability
            foreach (var abilityName in data.PurchasedAbilities)
            {
                // Get the ability
                var ability = GetAbility(abilityName);
                
                // Purchase the ability if it isn't already purchased
                if (ability == null || _purchasedAbilities.Contains(ability)) continue;
                _purchasedAbilities.Add(ability);
            }
        }
        
        public void TriggerAbilityDisplay(AbilityEffect abilityEffect)
        {
            OnAbilityPurchased?.Invoke(abilityEffect);
        }

        public void SaveData(GameData data)
        {
            // Save the purchased abilities
            data.PurchasedAbilities.Clear();
            foreach (var ability in _purchasedAbilities)
            {
                data.PurchasedAbilities.Add(ability.name);
            }
        }

        public bool FirstLoad()
        {
            return true;
        }
    }
}