using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DecreaseFireRate", menuName = "AbilityEffects/DecreaseFireRate")]
public class DecreaseFireRate : AbilityEffect
{
    public override void Apply(GameObject target)
    {
        PlayerController.Instance.CooldownPeriod += 0.2f;
    }
    
    public override void Unapply(GameObject target)
    {
        PlayerController.Instance.CooldownPeriod -= 0.2f;
    }
    
    public override bool IsUpgrade()
    {
        return false;
    }
}
