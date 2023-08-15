using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AddHeart", menuName = "AbilityEffects/AddHeart")]
public class AddHeart : AbilityEffect
{
    public override void Apply(GameObject target)
    {
        target.GetComponent<Health>().MaxHealth += 4;
        
        // Heal the player by 4
        target.GetComponent<Health>().Heal(4);
        
        // If the target is the player, update the UI
        if (target.CompareTag("Player"))
        {
            target.GetComponent<Health>().AddHeart();
        }
    }
    
    public override void Unapply(GameObject target)
    {
        target.GetComponent<Health>().MaxHealth -= 4;
    }
    
    public override bool IsUpgrade()
    {
        return true;
    }
}
