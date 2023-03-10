using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AddHeart", menuName = "AbilityEffects/AddHeart")]
public class AddHeart : AbilityEffect
{
    public override void Apply(GameObject target)
    {
        target.GetComponent<Health>().MaxHealth += 2;
    }
}
