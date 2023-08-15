using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Runtime.Abilities
{
    public class AbilityManager : MonoBehaviour
    {
        // This is a singleton that manages the abilities.
        // It contains pools of abilities that pertain to the floor the player is on.
        // It also contains a list of all abilities.
        // It is responsible for giving the player a random ability.

        public static AbilityManager Instance;
        public GameObject abilityInformation;

        [SerializeField] private List<AbilityEffect> forestAbilities = new();
        [SerializeField] private List<AbilityEffect> _purchasedAbilities = new();
        
        private void Awake()
        {
            Instance = this;
        }
        
        public AbilityEffect GetRandomAbility()
        {
            // Get the current floor from the GameManager
            var floor = GameManager.Instance.currentWorldName;
            
            // Get a random ability from the list of abilities for the current floor
            return floor switch
            {
                "Forest" => forestAbilities[Random.Range(0, forestAbilities.Count)],
                _ => null
            };
        }
        
        public void PurchaseAbility(AbilityEffect abilityEffect)
        {
            // Add the ability to the list of purchased abilities
            _purchasedAbilities.Add(abilityEffect);
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
            Invoke(nameof(DisableAbilityInformation), 2f);
        }
        
        private void DisableAbilityInformation()
        {
            abilityInformation.SetActive(false);
        }
    }
}