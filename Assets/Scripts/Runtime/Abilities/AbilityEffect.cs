using UnityEngine;

public abstract class AbilityEffect : ScriptableObject
{
    // Store the icon image
    public Sprite icon;
    
    public string description;
    public abstract void Apply(GameObject target);
    public abstract bool IsUpgrade();
}
