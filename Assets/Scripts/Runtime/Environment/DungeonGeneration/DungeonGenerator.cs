using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public DungeonGenerationData dungeonData;
    private List<Vector2Int> _dungeonRooms;
    private static List<DungeonGenerationData.RoomData> _roomData;

    private void Start()
    {
        // Make a deep copy of the room data
        _roomData = dungeonData.roomData.Select(room => new DungeonGenerationData.RoomData
        {
            roomName = room.roomName,
            probability = room.probability,
            probabilityModifier = room.probabilityModifier,
            singleRoom = room.singleRoom
        }).ToList();
        
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
            // Get a room name from the list of rooms based on the probability
            var roomName = GetRoomName();
                
            // Spawn the room
            RoomController.Instance.LoadRoom(roomName, room.x, room.y);
        }
    }
    
    private static string GetRoomName()
    {
        var totalProbability = 0;
        foreach (var room in _roomData)
        {
            totalProbability += room.probability;
        }
    
        var random = Random.Range(0, totalProbability);
        var currentProbability = 0;
        foreach (var room in _roomData)
        {
            currentProbability += room.probability;
            if (random < currentProbability)
            {
                var roomName = room.roomName;
                // If the room is a single room, remove it from the list
                if (room.singleRoom)
                {
                    _roomData.Remove(room);
                }
                
                // Update the probability of the other rooms
                foreach (var otherRoom in _roomData)
                {
                    otherRoom.probability = Mathf.RoundToInt(otherRoom.probability * otherRoom.probabilityModifier);
                    Debug.Log("Room: " + otherRoom.roomName + " Probability: " + otherRoom.probability);
                }

                return roomName;
            }
        }
    
        return null;
    }
}
