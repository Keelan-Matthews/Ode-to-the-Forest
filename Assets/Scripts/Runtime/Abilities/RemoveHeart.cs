using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RemoveHeart", menuName = "AbilityEffects/RemoveHeart")]
public class RemoveHeart : AbilityEffect
{
    public override void Apply(GameObject target)
    {
        PlayerController.Instance.GetComponent<Health>().MaxHealth -= 4;
        
        // Heal the player by 4
        PlayerController.Instance.GetComponent<Health>().TakeDamage(4);
        
        // If the target is the player, update the UI
        if (target.CompareTag("Player"))
        {
            PlayerController.Instance.GetComponent<Health>().RemoveHeart();
        }
    }
    
    public override void Unapply(GameObject target)
    {
        PlayerController.Instance.GetComponent<Health>().MaxHealth += 4;
    }
    
    public override bool IsUpgrade()
    {
        return false;
    }
}
