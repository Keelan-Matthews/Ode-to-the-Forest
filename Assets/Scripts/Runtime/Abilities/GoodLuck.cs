using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GoodLuck", menuName = "AbilityEffects/GoodLuck")]
public class GoodLuck : AbilityEffect
{

    public override void Apply(GameObject target)
    {
        // IMPLEMENT
        
    }

    public override void Unapply(GameObject target)
    {
    }

    public override bool IsUpgrade()
    {
        return true;
    }
}
