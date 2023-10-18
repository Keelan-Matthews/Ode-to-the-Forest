using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
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
        
        [Header("Ability Info references")]
        [SerializeField] private TextMeshProUGUI abilityName;
        [SerializeField] private TextMeshProUGUI abilityDescription;
        [SerializeField] private Image abilityIcon;
        
        public static event Action OnAllAbilitiesPurchased;
        public static event Action<AbilityEffect> OnAbilityPurchased;
        
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (abilityInformation == null) return;
            abilityInformation.SetActive(false);
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
            int maximumRerolls = 50;
            do
            {
                ability = floor switch
                {
                    "Forest" => forestAbilities[Random.Range(0, forestAbilities.Count)],
                    _ => null
                };
                maximumRerolls--;
            } while (permaSeedAbilities.Contains(ability) && maximumRerolls > 0);
            
            
            // If Clover seed is active and the ability is a downgrade, reroll
            // if (permaSeedAbilities.Contains(PermaSeedManager.Instance.GetSpecificPermaSeed("Clover").abilityEffect) && !ability.IsUpgrade() && !_rerolled)
            // {
            //     _rerolled = true;
            //     return GetRandomAbility();
            // }
            
            // Return the ability
            return ability;
        }
        
        public string[] GetAbilityNames()
        {
            // Get the current floor from the GameManager
            var floor = GameManager.Instance.currentWorldName;
            
            // Get the list of abilities for the current floor
            var abilities = floor switch
            {
                "Forest" => forestAbilities,
                _ => null
            };

            // Get the names of the abilities
            var abilityNames = new string[abilities.Count];
            for (var i = 0; i < abilities.Count; i++)
            {
                abilityNames[i] = abilities[i].abilityName;
            }

            // Return the ability names
            return abilityNames;
        }

        public AbilityEffect GetObeliskAbility()
        {
            var ability = GetRandomAbility();
            
            // If the ability is "Vase" reroll until it isn't
            while (ability.abilityName == "Vase")
            {
                ability = GetRandomAbility();
            }
            
            return ability;
        }
        
        public void PurchaseAbility(AbilityEffect abilityEffect)
        {
            if (!GetPurchasedAbilities().Contains(abilityEffect))
            {
                // Add the ability to the list of purchased abilities
                _purchasedAbilities.Add(abilityEffect);
            }

            // Invoke the OnAbilityPurchased event
            OnAbilityPurchased?.Invoke(abilityEffect);
        }
        
        public void PurchaseAllAbilities()
        {
            // Get the current floor from the GameManager
            var floor = GameManager.Instance.currentWorldName;
            
            // Get the list of abilities for the current floor
            var abilities = floor switch
            {
                "Forest" => forestAbilities,
                _ => null
            };

            // Add all the abilities to the list of purchased abilities
            foreach (var ability in abilities)
            {
                if (!GetPurchasedAbilities().Contains(ability))
                {
                    _purchasedAbilities.Add(ability);
                }
            }
            
            OnAllAbilitiesPurchased?.Invoke();
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
                "Forest" => forestAbilities.Find(ability =>
                {
                    var abilityName1 = ability.abilityName;
                    return ability.abilityName == abilityName;
                }),
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
        public void DisplayAbilityStats(AbilityEffect abilityEffect, bool autohide = true)
        {
            abilityInformation.SetActive(false);
            abilityInformation.SetActive(true);
            AudioManager.PlaySound(AudioManager.Sound.ShowMenu, transform.position);
            
            // Set the icon to the ability's icon
            abilityIcon.sprite = abilityEffect.icon;
            // Set the text to the ability's name
            abilityName.text = abilityEffect.abilityName;
            // Set the text to the ability's description
            abilityDescription.text = abilityEffect.description;
            
            // Force canvas update
            Canvas.ForceUpdateCanvases();
            var layoutGroups = abilityInformation.GetComponentsInChildren<HorizontalLayoutGroup>();
            // Disable the layout groups and then re-enable them to force the text to wrap
            foreach (var layoutGroup in layoutGroups)
            {
                layoutGroup.enabled = false;
                layoutGroup.enabled = true;
            }
            
            abilityInformation.GetComponent<Animator>().SetTrigger("Show");
            
            // SetActive to false after 2 seconds
            if (autohide)
            {
                StartCoroutine(HideAbilityStats());
            }
        }

        private IEnumerator HideAbilityStats()
        {
            // AFter 3 seconds, set the ability information to inactive
            yield return new WaitForSeconds(3);
            abilityInformation.SetActive(false);
            abilityInformation.GetComponent<Animator>().SetTrigger("Hide");
        }
        
        public void HideAbilityStatsNow()
        {
            abilityInformation.SetActive(false);
            abilityInformation.GetComponent<Animator>().SetTrigger("Hide");
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
            if (ScenesManager.Instance.currentSceneName == "ForestMain" &&
                PlayerController.Instance.GetComponent<Health>().HealthValue > 0) return;

            // Save the purchased abilities
            data.PurchasedAbilities.Clear();
            foreach (var ability in _purchasedAbilities)
            {
                data.PurchasedAbilities.Add(ability.abilityName);
            }
        }
        
        public void ResetSaveData()
        {
            // Clear the list of purchased abilities
            _purchasedAbilities.Clear();
        }

        public bool FirstLoad()
        {
            return true;
        }
        
        public bool IsActive()
        {
            return gameObject.activeSelf;
        }
    }
}