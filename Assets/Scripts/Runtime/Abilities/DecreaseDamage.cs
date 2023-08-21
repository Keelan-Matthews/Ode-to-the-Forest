using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DecreaseDamage", menuName = "AbilityEffects/DecreaseDamage")]
public class DecreaseDamage : AbilityEffect
{
    private const int Decreaser = 2;

    public override void Apply(GameObject target)
    {
        // Decrease the bullet damage
        PlayerController.Instance.FireDamage -= Decreaser;
        
        // If it is less than 1, set it to 1
        if (PlayerController.Instance.FireDamage < 1)
        {
            PlayerController.Instance.FireDamage = 1;
        }
    }

    public override void Unapply(GameObject target)
    {
        // Increase the bullet damage
        PlayerController.Instance.FireDamage += Decreaser;
    }

    public override bool IsUpgrade()
    {
        return false;
    }
}
