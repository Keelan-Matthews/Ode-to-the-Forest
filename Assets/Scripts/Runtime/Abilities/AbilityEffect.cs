using UnityEngine;

public abstract class AbilityEffect : ScriptableObject
{
    public abstract void Apply(GameObject target);
    public abstract bool IsUpgrade();
}
