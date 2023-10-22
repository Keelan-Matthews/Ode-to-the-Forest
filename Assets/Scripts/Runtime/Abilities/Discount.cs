using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Discount", menuName = "AbilityEffects/Discount")]
public class Discount : AbilityEffect
{
    public override void Apply(GameObject target)
    {
        GameManager.Instance.TriggerDiscount(1);
    }

    public override void Unapply(GameObject target)
    {
        GameManager.Instance.TriggerDiscount(-1);
    }

    public override bool IsUpgrade()
    {
        return true;
    }
}
