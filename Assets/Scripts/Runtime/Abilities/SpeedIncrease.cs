using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpeedIncrease", menuName = "AbilityEffects/SpeedIncrease")]
public class SpeedIncrease : AbilityEffect
{
    private int speedIncrease = 1;
    public override void Apply(GameObject target)
    {
        target.GetComponent<PlayerController>().Speed += speedIncrease;
    }
    
    public override bool IsUpgrade()
    {
        return true;
    }
}
