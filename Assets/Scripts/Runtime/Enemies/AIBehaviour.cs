using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIBehaviour : ScriptableObject
{
    public abstract void Think(BehaviourController bc);
}
