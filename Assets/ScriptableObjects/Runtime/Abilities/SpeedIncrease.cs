using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedIncrease : AbilityEffect
{
    [SerializeField] private int speedIncrease = 8;
    public override void Apply(GameObject target)
    {
        target.GetComponent<PlayerController>().Speed += speedIncrease;
    }
}
