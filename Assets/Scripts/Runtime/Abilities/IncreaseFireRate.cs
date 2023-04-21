using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AddHeart", menuName = "AbilityEffects/IncreaseFireRate")]
public class IncreaseFireRate : AbilityEffect
{
    public override void Apply(GameObject target)
    {
        target.GetComponent<PlayerController>().CooldownPeriod -= 0.2f;
    }
}
