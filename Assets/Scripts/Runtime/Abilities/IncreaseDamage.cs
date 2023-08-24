using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IncreaseDamage", menuName = "AbilityEffects/IncreaseDamage")]
public class IncreaseDamage : AbilityEffect
{
    private const int Increaser = 2;

    public override void Apply(GameObject target)
    {
        // Decrease the bullet damage
        PlayerController.Instance.FireDamage += Increaser;
    }

    public override void Unapply(GameObject target)
    {
        // Increase the bullet damage
        PlayerController.Instance.FireDamage -= Increaser;
    }

    public override bool IsUpgrade()
    {
        return true;
    }
}
