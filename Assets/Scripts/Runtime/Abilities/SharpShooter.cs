using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SharpShooter", menuName = "AbilityEffects/SharpShooter")]
public class SharpShooter : AbilityEffect
{
    public override void Apply(GameObject target)
    {
        target.GetComponent<BulletController>().isSharpshooter = true;
    }

    public override void Unapply(GameObject target)
    {
        target.GetComponent<BulletController>().isSharpshooter = false;
    }

    public override bool IsUpgrade()
    {
        return true;
    }
}
