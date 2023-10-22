using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ClearSkies", menuName = "AbilityEffects/ClearSkies")]
public class ClearSkies : AbilityEffect
{
    public override void Apply(GameObject target)
    {
        GameManager.Instance.IsClearSkies = true;
    }

    public override void Unapply(GameObject target)
    {
        GameManager.Instance.IsClearSkies = false;
    }

    public override bool IsUpgrade()
    {
        return true;
    }
}
