using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AddHeart", menuName = "AbilityEffects/Scattershot")]
public class Scattershot : AbilityEffect
{
    public override void Apply(GameObject target)
    {
        target.GetComponent<PlayerController>().IsScattershot = true;
        
        // Divide the bullets damage by 2
        if (PlayerController.Instance.FireDamage > 1)
            PlayerController.Instance.FireDamage /= 2;
    }
    
    public override bool IsUpgrade()
    {
        return true;
    }
}
