using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MinimapSeed", menuName = "PermaSeeds/MinimapSeed")]
public class MinimapSeed : PermaSeed
{
    public override void Apply()
    {
        MinimapManager.Instance.showMinimap = true;
    }
    
    public override void Remove()
    {
        MinimapManager.Instance.showMinimap = false;
    }

    public override bool IsGrown()
    {
        return essenceGiven >= essenceRequired;
    }
}
