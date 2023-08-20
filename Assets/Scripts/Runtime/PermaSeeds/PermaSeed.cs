using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PermaSeed : ScriptableObject
{
    public Sprite icon;
    public string seedName;
    public AbilityEffect abilityEffect;
    public RuntimeAnimatorController animatorController;
    protected bool isGrown;
    public abstract AbilityEffect GetAbilityEffect();
    public abstract void Remove();
    public abstract bool IsGrown();
    
    public abstract bool Grow(int essence);
    
    // The amount of essence required to grow the seed
    public int essenceRequired;
    
    // Set the seedName and icon to match the AbilityEffect
    public void SetSeedNameAndIcon()
    {
        seedName = abilityEffect.abilityName;
    }
}
