using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourController : MonoBehaviour
{
    [SerializeField] private AIBehaviour aiBehaviour;

    // Update is called once per frame
    void Update()
    {
        if (aiBehaviour == null) return;
        aiBehaviour.Think(this);
    }
}
