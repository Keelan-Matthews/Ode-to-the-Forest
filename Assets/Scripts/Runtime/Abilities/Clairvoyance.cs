using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Clairvoyance", menuName = "AbilityEffects/Clairvoyance")]
public class Clairvoyance : AbilityEffect
{

    public override void Apply(GameObject target)
    {
        MinimapManager.Instance.EnableAllRooms();
    }

    public override void Unapply(GameObject target)
    {
    }

    public override bool IsUpgrade()
    {
        return true;
    }
}
