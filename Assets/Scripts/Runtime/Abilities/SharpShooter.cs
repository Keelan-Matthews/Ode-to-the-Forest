using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SharpShooter", menuName = "AbilityEffects/SharpShooter")]
public class SharpShooter : AbilityEffect
{
    public override void Apply(GameObject target)
    {
        PlayerController.Instance.isSharpShooter = true;
    }

    public override void Unapply(GameObject target)
    {
        PlayerController.Instance.isSharpShooter = false;
    }

    public override bool IsUpgrade()
    {
        return true;
    }
}
