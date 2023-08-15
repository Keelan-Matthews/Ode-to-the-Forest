using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Corruption", menuName = "AbilityEffects/Corruption")]
public class Corruption : AbilityEffect
{
    public override void Apply(GameObject target)
    {
        PlayerController.Instance.isCorrupted = true;
    }

    public override void Unapply(GameObject target)
    {
        PlayerController.Instance.isCorrupted = false;
    }

    public override bool IsUpgrade()
    {
        return false;
    }
}
