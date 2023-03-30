using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWander", menuName = "AIBehaviour/Wander")]
public class Wander : AIBehaviour
{
    public override void Think(BehaviourController bc)
    {
        var movement = bc.gameObject.GetComponent<EnemyController>();
        if (movement)
        {
            //Make the enemy move in a random, believable direction
            movement.MoveTowardsTarget(movement.transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0));
        }
    }
}
