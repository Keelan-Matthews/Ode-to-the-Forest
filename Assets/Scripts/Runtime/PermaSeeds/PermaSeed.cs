using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PermaSeed : ScriptableObject
{
    protected bool isGrown;
    public abstract void Apply();
    public abstract void Remove();
    public abstract bool IsGrown();
    
    public abstract bool Grow(int essence);
    
    // The amount of essence required to grow the seed
    public int essenceRequired;
}
