using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public DungeonGenerationData dungeonData;
    private List<Vector2Int> _dungeonRooms;
    
    private void Start()
    {
        _dungeonRooms = DungeonCrawlerController.GenerateDungeon(dungeonData);
        SpawnRooms(_dungeonRooms);
    }
    
    private static void SpawnRooms(IEnumerable<Vector2Int> rooms)
    {
        // Spawn the start room
        RoomController.Instance.LoadRoom("Start", 0, 0);
        
        // Spawn the rest of the rooms
        foreach (var room in rooms)
        {
            RoomController.Instance.LoadRoom(RoomController.Instance.GetRandomRoomName(), room.x, room.y);
        }
    }
}
