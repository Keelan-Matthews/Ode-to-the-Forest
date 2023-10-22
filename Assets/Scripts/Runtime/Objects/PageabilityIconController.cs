using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PageabilityIconController : MonoBehaviour
{

    public AbilityEffect abilityEffect;
    public bool unlocked;

    public void DisplayDescription()
    {
        if (!unlocked) return;
        // Find the "Almanac" tag
        var almanac = GameObject.FindGameObjectWithTag("Almanac");
        // Get the almanac controller
        var almanacController = almanac.GetComponent<AlmanacController>();
        almanacController.DisplayAbilityStats(abilityEffect);
    }
    
    public void HideDescription()
    {
        // Find the "Almanac" tag
        var almanac = GameObject.FindGameObjectWithTag("Almanac");
        // Get the almanac controller
        var almanacController = almanac.GetComponent<AlmanacController>();
        almanacController.DisableAbilityInformation();
    }
}
