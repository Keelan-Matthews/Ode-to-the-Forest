using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilitySeedHolder", menuName = "PermaSeeds/AbilitySeedHolder")]
public class AbilitySeedHolder : PermaSeed
{
    public override AbilityEffect GetAbilityEffect()
    {
        return abilityEffect;
    }
    
    public override void Remove()
    {
        // Unapply the AddHeart ability effect
        abilityEffect.Unapply(PlayerController.Instance.gameObject);
    }

    public override bool IsGrown()
    {
        return isGrown;
    }
    
    public override bool Grow(int essence)
    {
        if (essence < essenceRequired) return false;
        isGrown = true;
        return true;
    }
}
