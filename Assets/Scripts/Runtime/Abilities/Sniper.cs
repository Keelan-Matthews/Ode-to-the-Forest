using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sniper", menuName = "AbilityEffects/Sniper")]
public class Sniper : AbilityEffect
{
    public override void Apply(GameObject target)
    {
        PlayerController.Instance.CooldownPeriod += 0.4f;
        // Increase range
        PlayerController.Instance.BulletRange += 2f;
        // Increase damage
        PlayerController.Instance.FireDamage += 4;
    }
    
    public override void Unapply(GameObject target)
    {
        PlayerController.Instance.CooldownPeriod -= 0.4f;
        // Decrease range
        PlayerController.Instance.BulletRange -= 2f;
        // Decrease damage
        PlayerController.Instance.FireDamage -= 4;
    }
    
    public override bool IsUpgrade()
    {
        return true;
    }
}
