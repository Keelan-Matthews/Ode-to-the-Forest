using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AddHeart", menuName = "AbilityEffects/IncreaseFireRate")]
public class IncreaseFireRate : AbilityEffect
{
    public override void Apply(GameObject target)
    {
        target.GetComponent<PlayerController>().CooldownPeriod -= 0.2f;
        
        // Decrease knockback
        target.GetComponent<KnockbackFeedback>().SetKnockback(2f);
    }
    
    public override void Unapply(GameObject target)
    {
        target.GetComponent<PlayerController>().CooldownPeriod += 0.2f;
        
        // Increase knockback
        target.GetComponent<KnockbackFeedback>().SetKnockback(4f);
    }
    
    public override bool IsUpgrade()
    {
        return true;
    }
}
