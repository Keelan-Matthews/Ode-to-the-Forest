using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonCrawler : MonoBehaviour
{
    private Vector2Int Position { get; set; }
    public DungeonCrawler(Vector2Int startPos)
    {
        Position = startPos;
    }

    public Vector2Int Move(Dictionary<Direction, Vector2Int> directionMovementMap)
    {
        // Generate position of the next room
        var toMove = (Direction) Random.Range(0, directionMovementMap.Count);
        var tempPosition = directionMovementMap[toMove];
        
        // Determine if new position of room will connect to more than one room
        var connections = 0;
        foreach (var direction in directionMovementMap)
        {
            if (DungeonCrawlerController.VisitedRooms.Contains(tempPosition + direction.Value))
            {
                connections++;
            }
        }

        // If the new position will connect to more than one room, generate a random weighted chance to regenerate the toMove 
        // so that the crawler will not move in that direction
        if (connections > 1)
        {
            var chance = Random.Range(0, 100);
            if (chance < 70)
            {
                var previousToMove = toMove;
                while (previousToMove == toMove)
                {
                    toMove = (Direction) Random.Range(0, directionMovementMap.Count);
                }
                tempPosition = directionMovementMap[toMove];
            }
        }
        
        Position += tempPosition;
        return Position;
    }
}
