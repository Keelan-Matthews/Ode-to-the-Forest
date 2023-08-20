using System.Collections;
using System.Collections.Generic;
using Runtime.Abilities;
using UnityEngine;
using UnityEngine.Serialization;

public class TraderController : MonoBehaviour
{
    [SerializeField] private List<GameObject> pedestals = new();
    
    private readonly List<AbilityEffect> _abilities = new();
    
    // On awake, add a different, random ability to each pedestal
    
    private void Awake()
    {
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
            
            // Try get a random ability that the player doesn't already have
            AbilityEffect ability;
            do
            {
                ability = AbilityManager.Instance.GetRandomAbility();
            } while (permaSeedAbilities.Contains(ability) || _abilities.Contains(ability));

            // Add the ability to the list
            _abilities.Add(ability);

            // Set the ability on the pedestal
            pedestalController.SetAbilityEffect(ability);
            
            // remove the ability from the ability manager
            AbilityManager.Instance.RemoveAbility(ability.name);
        }

    }
}
