using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GlassCannon", menuName = "AbilityEffects/GlassCannon")]
public class GlassCannon : AbilityEffect
{
    public override void Apply(GameObject target)
    {
        PlayerController.Instance.GetComponent<Health>().MaxHealth -= 11;
        
        // Increase the players damage and fire rate
        PlayerController.Instance.FireDamage += 4;
        PlayerController.Instance.CooldownPeriod -= 0.15f;
        
        // Decrease knockback
        target.GetComponent<KnockbackFeedback>().SetKnockback(2f);
        
        // Heal the player fully
        PlayerController.Instance.GetComponent<Health>().Heal(11);
        
        // If the target is the player, update the UI
        if (target.CompareTag("Player"))
        {
            PlayerController.Instance.GetComponent<Health>().RemoveHeart();
        }
    }
    
    public override void Unapply(GameObject target)
    {
        PlayerController.Instance.GetComponent<Health>().MaxHealth += 11;
    }
    
    public override bool IsUpgrade()
    {
        return false;
    }
}
