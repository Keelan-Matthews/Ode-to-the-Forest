using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sunburst", menuName = "AbilityEffects/Sunburst")]
public class Sunburst : AbilityEffect
{
    private const float Multiplier = 1.5f;

    public override void Apply(GameObject target)
    {
        RoomController.Instance.IncreaseSunlightRadius();
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
