using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SellYourSoul", menuName = "AbilityEffects/SellYourSoul")]
public class SellYourSoul : AbilityEffect
{
    public override void Apply(GameObject target)
    {
        GameManager.Instance.TriggerSellYourSoul();
    }

    public override void Unapply(GameObject target)
    {
        return;
    }

    public override bool IsUpgrade()
    {
        return false;
    }
}
