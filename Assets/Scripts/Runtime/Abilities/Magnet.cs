using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Magnet", menuName = "AbilityEffects/Magnet")]
public class Magnet : AbilityEffect
{
    public override void Apply(GameObject target)
    {
        target.GetComponent<PlayerController>().EnableEssenceMagnet();
    }

    public override void Unapply(GameObject target)
    {
        target.GetComponent<PlayerController>().DisableEssenceMagnet();
    }

    public override bool IsUpgrade()
    {
        return true;
    }
}
