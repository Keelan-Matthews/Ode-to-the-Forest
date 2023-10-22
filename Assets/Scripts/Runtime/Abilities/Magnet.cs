using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Magnet", menuName = "AbilityEffects/Magnet")]
public class Magnet : AbilityEffect
{
    public override void Apply(GameObject target)
    {
        PlayerController.Instance.EnableEssenceMagnet();
    }

    public override void Unapply(GameObject target)
    {
        PlayerController.Instance.DisableEssenceMagnet();
    }

    public override bool IsUpgrade()
    {
        return true;
    }
}
