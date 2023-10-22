using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RemoveHeart", menuName = "AbilityEffects/RemoveHeart")]
public class RemoveHeart : AbilityEffect
{
    public override void Apply(GameObject target)
    {
        var remainingHealth = 12 - PlayerController.Instance.GetComponent<Health>().HealthValue;
        PlayerController.Instance.GetComponent<Health>().MaxHealth -= 4;
        // If the player has 8 or less remaining health, just remove a heart and don't take damage,
        // otherwise take damage equivalent to the remaining health
        if (remainingHealth > 8)
        {
            PlayerController.Instance.GetComponent<Health>().TakeDamage(remainingHealth);
        }

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
