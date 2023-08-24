using System;
using System.Collections;
using System.Collections.Generic;
using Runtime.Abilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PageController : MonoBehaviour
{
    private List<AbilityEffect> _abilityEffects = new();
    private List<AbilityEffect> _purchasedAbilities = new();
    public int pageNumber;
    public GameObject abilityPagePrefab;
    // Start is called before the first frame update
    void Start()
    {
        _abilityEffects = AbilityManager.Instance.GetAbilities();
        _purchasedAbilities = AbilityManager.Instance.GetPurchasedAbilities();
        
        // If page is 1, go from ability 1 to halfway,
        // if page is 2, go from halfway to end.
        var start = pageNumber == 1 ? 0 : _abilityEffects.Count / 2;
        var end = pageNumber == 1 ? _abilityEffects.Count / 2 : _abilityEffects.Count;
        
        for (var i = start; i < end; i++)
        {
            var abilityPage = Instantiate(abilityPagePrefab, transform);
            abilityPage.GetComponentInChildren<Image>().sprite = _abilityEffects[i].icon;
            
            // Set the Image to be black if it is not purchased
            if (!_purchasedAbilities.Contains(_abilityEffects[i]))
            {
                abilityPage.GetComponentInChildren<Image>().color = new Color(0.1f, 0.1f, 0.1f, 1f);
            }
            else
            {
                abilityPage.GetComponentInChildren<TextMeshProUGUI>().text = _abilityEffects[i].abilityName;
            }
        }
    }
}
