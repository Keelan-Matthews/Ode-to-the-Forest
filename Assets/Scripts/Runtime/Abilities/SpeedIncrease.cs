using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpeedIncrease", menuName = "AbilityEffects/SpeedIncrease")]
public class SpeedIncrease : AbilityEffect
{
    [SerializeField] private int speedIncrease = 8;
    public override void Apply(GameObject target)
    {
        target.GetComponent<PlayerController>().Speed += speedIncrease;
    }
    
    public override bool IsUpgrade()
    {
        return true;
    }
}
