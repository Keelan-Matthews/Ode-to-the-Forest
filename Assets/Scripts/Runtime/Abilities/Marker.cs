using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Marker", menuName = "AbilityEffects/Marker")]
public class Marker : AbilityEffect
{

    public override void Apply(GameObject target)
    {
        MinimapManager.Instance.EnableSpecialRooms();
    }

    public override void Unapply(GameObject target)
    {
    }

    public override bool IsUpgrade()
    {
        return true;
    }
}
