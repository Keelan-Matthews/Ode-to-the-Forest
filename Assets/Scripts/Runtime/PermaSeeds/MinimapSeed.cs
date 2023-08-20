using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MinimapSeed", menuName = "PermaSeeds/MinimapSeed")]
public class MinimapSeed : PermaSeed
{
    public override AbilityEffect GetAbilityEffect()
    {
        return abilityEffect;
    }

    public override void Remove()
    {
        MinimapManager.Instance.showMinimap = false;
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
