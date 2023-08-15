using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpeedDecrease", menuName = "AbilityEffects/SpeedDecrease")]
public class SpeedDecrease : AbilityEffect
{
    private int speedDecrease = 2;
    public override void Apply(GameObject target)
    {
        PlayerController.Instance.Speed -= speedDecrease;
    }
    
    public override void Unapply(GameObject target)
    {
        PlayerController.Instance.Speed += speedDecrease;
    }
    
    public override bool IsUpgrade()
    {
        return false;
    }
}
