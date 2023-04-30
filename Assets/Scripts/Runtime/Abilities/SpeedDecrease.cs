using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpeedDecrease", menuName = "AbilityEffects/SpeedDecrease")]
public class SpeedDecrease : AbilityEffect
{
    [SerializeField] private int speedDecrease = 1;
    public override void Apply(GameObject target)
    {
        target.GetComponent<PlayerController>().Speed -= speedDecrease;
    }
    
    public override bool IsUpgrade()
    {
        return false;
    }
}
