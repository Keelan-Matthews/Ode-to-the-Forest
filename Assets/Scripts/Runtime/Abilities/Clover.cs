using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Clover", menuName = "AbilityEffects/Clover")]
public class Clover : AbilityEffect
{
    private const float Multiplier = 2f;

    public override void Apply(GameObject target)
    {
        GameManager.Instance.AddClover(Multiplier);
    }

    public override void Unapply(GameObject target)
    {
        GameManager.Instance.RemoveClover(Multiplier);
    }

    public override bool IsUpgrade()
    {
        return true;
    }
}
