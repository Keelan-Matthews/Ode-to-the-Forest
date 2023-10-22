using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sunveil", menuName = "AbilityEffects/Sunveil")]
public class Sunveil : AbilityEffect
{
    private const float Multiplier = 1.5f;

    public override void Apply(GameObject target)
    {
        RoomController.Instance.DecreaseSunlightRadius();
    }

    public override void Unapply(GameObject target)
    {
        return;
    }

    public override bool IsUpgrade()
    {
        return false;
    }
}
