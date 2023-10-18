using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Vase", menuName = "AbilityEffects/Vase")]
public class Vase : AbilityEffect
{
    public override void Apply(GameObject target)
    {
        // Add the vase perma seed
        var seed = PermaSeedManager.Instance.GetSpecificPermaSeed("Vase");
        
        if (PermaSeedManager.Instance.HasSeed())
        {
            // Drop the old seed
            var oldSeedName = PermaSeedManager.Instance.GetStoredPermaSeed().seedName;
            GameManager.Instance.DropSpecificPermaSeed(PlayerController.Instance.transform.position, oldSeedName);
            PermaSeedManager.Instance.RemoveStoredPermaSeed();
        }
        
        // Add to inventory
        PermaSeedManager.Instance.AddPermaSeed(seed);
        GameManager.Instance.CanSpawnVase = false;
    }
    
    public override void Unapply(GameObject target)
    {
    }
    
    public override bool IsUpgrade()
    {
        return true;
    }
}
