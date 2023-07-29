using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWander", menuName = "AIBehaviour/Wander")]
public class Wander : AIBehaviour
{
    private int _roomWidth = 10;
    private int _roomHeight = 18;

    public override void Think(BehaviourController bc)
    {
        var movement = bc.gameObject.GetComponent<EnemyController>();

        if (movement)
        {
            // Check if the enemy has reached the destination or if it needs a new one
            if (movement.HasReachedDestination())
            {
                // Generate a new random destination within the room's bounds
                var newDestination = GetRandomPositionWithinRoom();

                // Set the new destination for the enemy to move towards
                movement.MoveTowardsTarget(newDestination);
            }
        }
    }

// Function to generate a random position within the bounds of the room
    private Vector2 GetRandomPositionWithinRoom()
    {
        float halfWidth = _roomWidth / 2;
        float halfHeight = _roomHeight / 2;

        var randomPosition = new Vector2(
            Random.Range(-halfWidth, halfWidth),
            Random.Range(-halfHeight, halfHeight)
        );

        return randomPosition;
    }
}