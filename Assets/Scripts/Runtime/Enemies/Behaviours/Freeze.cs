using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyFreeze", menuName = "AIBehaviour/Freeze")]
public class Freeze : AIBehaviour
{
    [SerializeField] private string targetTag = "Player";
    public override void Think(BehaviourController bc)
    {
        var target = GameObject.FindGameObjectWithTag(targetTag);
        if (!target) return;
        var movement = bc.gameObject.GetComponent<EnemyController>();
        if (movement)
        {
            movement.StopMoving();
        }
    }
}
