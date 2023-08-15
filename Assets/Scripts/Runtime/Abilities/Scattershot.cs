using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AddHeart", menuName = "AbilityEffects/Scattershot")]
public class Scattershot : AbilityEffect
{
    public override void Apply(GameObject target)
    {
        PlayerController.Instance.IsScattershot = true;
        
        // Divide the bullets damage by 2
        if (PlayerController.Instance.FireDamage > 1)
            PlayerController.Instance.FireDamage /= 2;
        
        // Increase the range slightly
        PlayerController.Instance.BulletRange += 0.1f;
    }
    
    public override void Unapply(GameObject target)
    {
        PlayerController.Instance.IsScattershot = false;
        
        // Multiply the bullets damage by 2
        PlayerController.Instance.FireDamage *= 2;
        
        // Decrease the range slightly
        PlayerController.Instance.BulletRange -= 0.1f;
    }
    
    public override bool IsUpgrade()
    {
        return true;
    }
}
