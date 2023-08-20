using System.Collections;
using System.Collections.Generic;
using Runtime.Abilities;
using UnityEngine;

public class AbilityDisplay : MonoBehaviour
{
    [SerializeField] private GameObject abilityUIPrefab;

    [SerializeField] private List<AbilityUIPrefab> abilityUIPrefabs;
    
    private void Awake()
    {
        abilityUIPrefabs = new List<AbilityUIPrefab>();
        AbilityManager.OnAbilityPurchased += DrawAbility;
    }
    
    private void OnDisable()
    {
        AbilityManager.OnAbilityPurchased -= DrawAbility;
    }
    
    private void OnDestroy()
    {
        ClearAbilities();
    }

    private void DrawAbility(AbilityEffect ability)
    {
        // Make a new ability UI prefab
        var newAbilityUI = Instantiate(abilityUIPrefab, transform);
        var abilityUIComponent = newAbilityUI.GetComponent<AbilityUIPrefab>();
        
        // Set the ability UI prefab's sprite
        abilityUIComponent.SetAbility(ability.abilityName);
        
        // Add the ability UI prefab to the list
        abilityUIPrefabs.Add(abilityUIComponent);
    }
    
    public void ClearAbilities()
    {
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        abilityUIPrefabs = new List<AbilityUIPrefab>();
    }
}
