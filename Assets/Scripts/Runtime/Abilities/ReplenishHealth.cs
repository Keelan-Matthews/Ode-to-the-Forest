using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ReplenishHealth", menuName = "AbilityEffects/ReplenishHealth")]
public class ReplenishHealth : AbilityEffect
{
    public override void Apply(GameObject target)
    {
        PlayerController.Instance.GetComponent<Health>().Heal(100);
    }
    
    public override void Unapply(GameObject target)
    {
        return;
    }
    
    public override bool IsUpgrade()
    {
        return true;
    }
}
