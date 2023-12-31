using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FreezePea", menuName = "AbilityEffects/FreezePea")]
public class FreezePea : AbilityEffect
{
    public override void Apply(GameObject target)
    {
        PlayerController.Instance.isFreezePea = true;
    }

    public override void Unapply(GameObject target)
    {
        PlayerController.Instance.isFreezePea = false;
    }

    public override bool IsUpgrade()
    {
        return true;
    }
}
