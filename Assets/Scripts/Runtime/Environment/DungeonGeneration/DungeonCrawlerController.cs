using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Up = 0,
    Left = 1,
    Down = 2,
    Right = 3
}

public class DungeonCrawlerController : MonoBehaviour
{
    public static readonly List<Vector2Int> VisitedRooms = new ();
    private static readonly Dictionary<Direction, Vector2Int> DirectionMovementMap = new ()
    {
        {Direction.Up, Vector2Int.up},
        {Direction.Left, Vector2Int.left},
        {Direction.Down, Vector2Int.down},
        {Direction.Right, Vector2Int.right}
    };

    public static List<Vector2Int> GenerateDungeon(DungeonGenerationData dungeonData, GameObject gameObject)
    {
        VisitedRooms.Clear();
        List<DungeonCrawler> crawlers = new ();

        // Create crawlers
        for (var i = 0; i < dungeonData.numberOfCrawlers; i++)
        {
            crawlers.Add(gameObject.AddComponent<DungeonCrawler>());
        }
        
        // Move crawlers
        var iterations = Random.Range(dungeonData.iterationMin, dungeonData.iterationMax);
        var count = 0;
        while (count < iterations * dungeonData.numberOfCrawlers)
        {
            foreach (var crawler in crawlers)
            {
                // Get new position
                var newPosition = crawler.Move(DirectionMovementMap);
                // Add to visited rooms if not already visited
                if (VisitedRooms.Contains(newPosition)) continue;
                VisitedRooms.Add(newPosition);
                count++;
            }
        }

        return VisitedRooms;
    }
}
