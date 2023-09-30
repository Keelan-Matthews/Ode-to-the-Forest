using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public DungeonGenerationData dungeonData;
    private List<Vector2Int> _dungeonRooms;
    private static List<DungeonGenerationData.RoomData> _roomData;
    private static int _iterations;

    private void Start()
    {
        if (!RoomController.Instance.generateDungeon) return;
        // Make a deep copy of the room data
        _roomData = dungeonData.roomData.Select(room => new DungeonGenerationData.RoomData
        {
            roomName = room.roomName,
            singleRoom = room.singleRoom
        }).ToList();

        _dungeonRooms = DungeonCrawlerController.GenerateDungeon(dungeonData, gameObject);
        // Spawn the rooms
        SpawnRooms(_dungeonRooms);
    }

    private static void SpawnRooms(IEnumerable<Vector2Int> rooms)
    {
        _iterations = 0;
        // Spawn the start room
        RoomController.Instance.LoadRoom("Start", 0, 0);
        MinimapManager.Instance.LoadMinimapRoom("Start", 0, 0);
        
        // Room count
        var vector2Ints = rooms as Vector2Int[] ?? rooms.ToArray();
        var numRooms = vector2Ints.Length;

        // Spawn the rest of the rooms
        foreach (var room in vector2Ints)
        {
            // Get a room name from the list of rooms based on the probability
            var roomName = GetRoomName(numRooms);
            
            _iterations++;
                
            // Spawn the room
            RoomController.Instance.LoadRoom(roomName, room.x, room.y);
            MinimapManager.Instance.LoadMinimapRoom(roomName, room.x, room.y);
        }
    }

    private static string GetRoomName(int numRooms)
    {
        var step = numRooms / 3;

        // If iterations is less than 1/3 of the number of rooms, spawn easy rooms or the vending machine
        if (_iterations <= step - 1)
        {
            return GetRoom("Easy", step, "VendingMachine");
        }

        if (_iterations < step * 2)
        {
            if (GameManager.Instance.deeperPortalSpawn)
            {
                return GetRoom("Medium", step, "ShrineOfYouth", "Collector");
            }

            return GetRoom("Medium", step, "Portal", "ShrineOfYouth");
        }

        if (_iterations > step * 3) GetRoom("Hard", step, "Trader");
        if (GameManager.Instance.deeperPortalSpawn)
        {
            return GetRoom("Hard", step, "Trader", "Portal");
        }
        
        return GetRoom("Hard", step, "Trader", "Collector");
    }

    private static string GetRoom(string roomName, int step, string specialRoom = null, string specialRoom2 = null)
    {
        if (specialRoom != null)
        {
            var specialRoomObject = _roomData.Find(room => room.roomName == specialRoom);

            if (specialRoomObject != null)
            {
                if (Random.Range(0, step) < _iterations || (_iterations == step - 1 && _roomData.Contains(specialRoomObject)))
                {
                    _roomData.Remove(specialRoomObject);
                    return specialRoomObject.roomName;
                }
            }
        }
        
        if (specialRoom2 != null)
        {
            var specialRoomObject = _roomData.Find(room => room.roomName == specialRoom2);
        
            if (specialRoomObject != null)
            {
                if (Random.Range(0, step) < _iterations || (_iterations == step - 2 && _roomData.Contains(specialRoomObject)))
                {
                    _roomData.Remove(specialRoomObject);
                    return specialRoomObject.roomName;
                }
            }
        }

        // Return a regular room
        var rooms = _roomData.FindAll(room => room.roomName.Contains(roomName));
        
        // If it is a hard room, there is a 40% chance it becomes an extreme room
        if (roomName == "Hard")
        {
            var random = Random.Range(0, 100);
            if (random < 40)
            {
                rooms = _roomData.FindAll(room => room.roomName.Contains("Extreme"));
            }
        }
        
        return rooms[Random.Range(0, rooms.Count)].roomName;
    }
}