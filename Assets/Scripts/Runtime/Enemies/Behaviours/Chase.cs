using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyChase", menuName = "AIBehaviour/Chase")]
public class Chase : AIBehaviour
{
    [SerializeField] private string targetTag = "Player";

    public override void Think(BehaviourController bc)
    {
        var target = GameObject.FindGameObjectWithTag(targetTag);
        if (target)
        {
            var movement = bc.gameObject.GetComponent<EnemyController>();
            if (movement)
            {
                movement.MoveTowardsTarget(target.transform.position);
            }
        }
    }
}
