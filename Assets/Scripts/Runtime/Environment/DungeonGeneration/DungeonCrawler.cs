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
        var toMove = (Direction) Random.Range(0, directionMovementMap.Count);
        Position += directionMovementMap[toMove];
        return Position;
    }
}
