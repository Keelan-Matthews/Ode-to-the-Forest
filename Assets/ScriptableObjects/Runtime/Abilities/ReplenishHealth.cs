using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ReplenishHealth", menuName = "AbilityEffects/ReplenishHealth")]
public class ReplenishHealth : AbilityEffect
{
    public override void Apply(GameObject target)
    {
        target.GetComponent<Health>().Heal(100);
    }
}
