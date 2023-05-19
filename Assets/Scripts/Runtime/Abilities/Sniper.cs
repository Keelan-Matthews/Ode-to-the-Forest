using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sniper", menuName = "AbilityEffects/Sniper")]
public class Sniper : AbilityEffect
{
    public override void Apply(GameObject target)
    {
        target.GetComponent<PlayerController>().CooldownPeriod += 0.4f;
        // Increase range
        target.GetComponent<PlayerController>().BulletRange += 2f;
        // Increase damage
        target.GetComponent<PlayerController>().FireDamage += 4;
    }
    
    public override bool IsUpgrade()
    {
        return true;
    }
}
