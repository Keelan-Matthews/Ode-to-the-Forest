using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GoodLuck", menuName = "AbilityEffects/GoodLuck")]
public class GoodLuck : AbilityEffect
{

    public override void Apply(GameObject target)
    {
        GameManager.Instance.goodLuck = true;
    }

    public override void Unapply(GameObject target)
    {
        GameManager.Instance.goodLuck = false;
    }

    public override bool IsUpgrade()
    {
        return true;
    }
}
