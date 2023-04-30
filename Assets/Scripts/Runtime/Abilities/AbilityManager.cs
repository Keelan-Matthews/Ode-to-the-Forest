using System.Collections.Generic;
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

        [SerializeField] private List<AbilityEffect> forestAbilities = new();
        
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
    }
}