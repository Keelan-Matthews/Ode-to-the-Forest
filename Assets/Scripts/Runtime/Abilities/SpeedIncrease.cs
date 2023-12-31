using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpeedIncrease", menuName = "AbilityEffects/SpeedIncrease")]
public class SpeedIncrease : AbilityEffect
{
    private int speedIncrease = 1;
    public override void Apply(GameObject target)
    {
        PlayerController.Instance.Speed += speedIncrease;
    }
    
    public override void Unapply(GameObject target)
    {
        PlayerController.Instance.Speed -= speedIncrease;
    }
    
    public override bool IsUpgrade()
    {
        return true;
    }
}
