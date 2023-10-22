using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Minimap", menuName = "AbilityEffects/Minimap")]
public class Minimap : AbilityEffect
{

    public override void Apply(GameObject target)
    {
        MinimapManager.Instance.showMinimap = true;
    }

    public override void Unapply(GameObject target)
    {
        MinimapManager.Instance.showMinimap = false;
    }

    public override bool IsUpgrade()
    {
        return true;
    }
}
