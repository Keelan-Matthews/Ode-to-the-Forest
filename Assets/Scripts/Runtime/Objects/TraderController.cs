using System;
using System.Collections;
using System.Collections.Generic;
using Runtime.Abilities;
using UnityEngine;
using UnityEngine.Serialization;

public class TraderController : MonoBehaviour
{
    [SerializeField] private List<GameObject> pedestals = new();
    
    private List<AbilityEffect> _abilities;
    
    // On awake, add a different, random ability to each pedestal
    
    private void Awake()
    {
        AbilityManager.OnAllAbilitiesPurchased += PopulatePedestals;
    }
    
    private void OnDisable()
    {
        AbilityManager.OnAllAbilitiesPurchased -= PopulatePedestals;
    }

    private void Start()
    {
        PopulatePedestals();
    }

    private void PopulatePedestals()
    {
        _abilities = new List<AbilityEffect>();
        foreach (var pedestal in pedestals)
        {
            var pedestalController = pedestal.GetComponent<PedestalController>();
            if (pedestalController == null) continue;
            
            var permaSeeds = PermaSeedManager.Instance.GetActiveSeeds();
            // get the abilities from the perma seeds
            var permaSeedAbilities = new List<AbilityEffect>();
            foreach (var permaSeed in permaSeeds)
            {
                permaSeedAbilities.Add(permaSeed.abilityEffect);
            }
            
            // If the player can get a Vase, make sure it spawns on the first pedestal
            if (GameManager.Instance.CanSpawnVase)
            {
                var vase = AbilityManager.Instance.GetAbility("Vase");
                if (vase != null)
                {
                    _abilities.Add(vase);
                    pedestalController.SetAbilityEffect(vase);
                    AbilityManager.Instance.RemoveAbility(vase.name);
                    
                    // Inflate the cost of this pedestal
                    pedestalController.InflateCost(5);
                    continue;
                }
            }
            
            // Try get a random ability that the player doesn't already have
            AbilityEffect ability;
            do
            {
                ability = AbilityManager.Instance.GetRandomAbility();
            } while (permaSeedAbilities.Contains(ability) || _abilities.Contains(ability) ||
                     ability.abilityName == "Vase");

            // Add the ability to the list
            _abilities.Add(ability);

            // Set the ability on the pedestal
            pedestalController.SetAbilityEffect(ability);
            
            // remove the ability from the ability manager
            AbilityManager.Instance.RemoveAbility(ability.name);
        }
    }
}
