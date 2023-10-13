using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BossSense", menuName = "AbilityEffects/BossSense")]
public class BossSense : AbilityEffect
{
    public override void Apply(GameObject target)
    {
        MinimapManager.Instance.SetBossRoomVisited();
    }
    
    public override void Unapply(GameObject target)
    {
    }
    
    public override bool IsUpgrade()
    {
        return true;
    }
}
