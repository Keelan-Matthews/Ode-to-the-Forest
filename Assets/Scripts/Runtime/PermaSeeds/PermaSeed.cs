using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PermaSeed : ScriptableObject
{
    public Sprite icon;
    public abstract void Apply();
    public abstract void Remove();
    public abstract bool IsGrown();
    
    // The amount of essence required to grow the seed
    public int essenceRequired;
    
    // the amount of essence the player has given to the seed
    public int essenceGiven;
}
