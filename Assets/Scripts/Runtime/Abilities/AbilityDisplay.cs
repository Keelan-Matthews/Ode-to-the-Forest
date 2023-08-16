using System.Collections;
using System.Collections.Generic;
using Runtime.Abilities;
using UnityEngine;

public class AbilityDisplay : MonoBehaviour
{
    [SerializeField] private GameObject abilityUIPrefab;
    
    List<AbilityUIPrefab> abilityUIPrefabs = new ();
    
    private void OnEnable()
    {
        AbilityManager.OnAbilityPurchased += DrawAbility;
    }
    
    private void OnDisable()
    {
        AbilityManager.OnAbilityPurchased -= DrawAbility;
    }
    
    private void Start()
    {
        ClearAbilities();
    }

    // private void AddAllAbilities()
    // {
    //     foreach (var ability in AbilityManager.Instance.GetAbilities())
    //     {
    //         DrawAbility(ability);
    //     }
    // }

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
    
    private void ClearAbilities()
    {
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        abilityUIPrefabs = new List<AbilityUIPrefab>();
    }
}
