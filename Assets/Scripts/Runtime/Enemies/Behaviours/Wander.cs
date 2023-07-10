using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWander", menuName = "AIBehaviour/Wander")]
public class Wander : AIBehaviour
{
    private Vector2 _destination = Vector2.zero;
    private bool _reachedDestination;
    private int _roomWidth = 14;
    private int _roomHeight = 22;
    
    public override void Think(BehaviourController bc)
    {
        var movement = bc.gameObject.GetComponent<EnemyController>();
        if (movement)
        {
            // Set the destination of the enemy controller to a random position
            // Then wait for the enemy to reach that position before setting
            // a new destination
            if (_reachedDestination)
            {
                // Ensure that the destination is within the bounds of the map
                do
                {
                    _destination = new Vector2(Random.Range(-_roomWidth / 2, _roomWidth / 2),
                        Random.Range(-_roomHeight / 2, _roomHeight / 2));
                } while (!IsPositionValid(_destination));
                
                Debug.Log("New destination: " + _destination);

                _reachedDestination = false;
            }
            else
            {
                movement.MoveTowardsTarget(_destination);
                if (Vector2.Distance(bc.gameObject.transform.position, _destination) < 0.1f)
                {
                    Debug.Log("Reached destination");
                    _reachedDestination = true;
                }
            }
        }
    }
    
    // This function checks if the given position is within the bounds of the map
    private bool IsPositionValid(Vector2 position)
    {
        return position.x > -_roomWidth / 2 && position.x < _roomWidth / 2 &&
               position.y > -_roomHeight / 2 && position.y < _roomHeight / 2;
    }
}
