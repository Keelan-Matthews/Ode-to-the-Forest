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
        // Make a deep copy of the room data
        _roomData = dungeonData.roomData.Select(room => new DungeonGenerationData.RoomData
        {
            roomName = room.roomName,
            probability = room.probability,
            probabilityModifier = room.probabilityModifier,
            singleRoom = room.singleRoom
        }).ToList();
        
        _dungeonRooms = DungeonCrawlerController.GenerateDungeon(dungeonData, gameObject);
        SpawnRooms(_dungeonRooms);
    }

    private static void SpawnRooms(IEnumerable<Vector2Int> rooms)
    {
        _iterations++;
        
        // Spawn the start room
        RoomController.Instance.LoadRoom("Start", 0, 0);
        
        // Room count
        var vector2Ints = rooms as Vector2Int[] ?? rooms.ToArray();
        var numRooms = vector2Ints.Length;
        
        // Spawn the rest of the rooms
        foreach (var room in vector2Ints)
        {
            // Get a room name from the list of rooms based on the probability
            var roomName = GetRoomName(numRooms);
                
            // Spawn the room
            RoomController.Instance.LoadRoom(roomName, room.x, room.y);
        }
    }
    
    private static string GetRoomName(int numRooms)
    {
        // If iterations is half the number of rooms, spawn the vending machine
        if (_iterations == numRooms/2)
        {
            var vendingMachine = _roomData.Find(room => room.roomName == "VendingMachine");
            if (vendingMachine != null)
            {
                _roomData.Remove(vendingMachine);
                return vendingMachine.roomName;
            }
        }

        // If iterations is 2 less than the number of rooms, spawn the trader
        if (_iterations == numRooms - 2)
        {
            var trader = _roomData.Find(room => room.roomName == "Trader");
            if (trader != null)
            {
                _roomData.Remove(trader);
                return trader.roomName;
            }
        }
        
        var totalProbability = _roomData.Sum(room => room.probability);

        var random = Random.Range(0, totalProbability);
        var currentProbability = 0;
        foreach (var room in _roomData)
        {
            currentProbability += room.probability;
            if (random >= currentProbability) continue;
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
            }

            return roomName;
        }
    
        return null;
    }
}
