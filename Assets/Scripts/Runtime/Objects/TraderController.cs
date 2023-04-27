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
            var ability = AbilityManager.Instance.GetRandomAbility();

            // If the ability is already in the list, get a new one
            while (_abilities.Contains(ability))
            {
                ability = AbilityManager.Instance.GetRandomAbility();
            }

            // Add the ability to the list
            _abilities.Add(ability);

            // Set the ability on the pedestal
            pedestalController.SetAbilityEffect(ability);
        }

    }
}
