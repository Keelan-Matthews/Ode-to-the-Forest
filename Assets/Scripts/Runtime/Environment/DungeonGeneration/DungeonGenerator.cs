using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public DungeonGenerationData dungeonData;
    private List<Vector2Int> _dungeonRooms;
    private static List<DungeonGenerationData.RoomData> _roomData;
    private static int _iterations = 0;

    private void Start()
    {
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
        // New approach, the first 1/3 of the dungeon will spawn easy rooms, the second 1/3 will spawn medium rooms,
        // and the last 1/3 will spawn hard rooms
        // The first 1/3 will have the vending machine, and the last 1/3 will have the trader
        
        // If iterations is less than 1/3 of the number of rooms, spawn easy rooms or the vending machine
        if (_iterations <= numRooms/3 - 1)
        {
            // Determine if the vending machine should be spawned
            var vendingMachine = _roomData.Find(room => room.roomName == "VendingMachine");
            if (vendingMachine != null)
            {
                // the probability of spawning is iterations / numRooms/3 chance
                if (Random.Range(0, numRooms/3) < _iterations)
                {
                    _roomData.Remove(vendingMachine);
                    return vendingMachine.roomName;
                }
                
                // If iterations is 1/3 of the number of rooms, spawn the vending machine
                if (_iterations == numRooms/3 - 1 && _roomData.Contains(vendingMachine))
                {
                    _roomData.Remove(vendingMachine);
                    return vendingMachine.roomName;
                }
            }
            
            // Return an easy room
            var easyRooms = _roomData.FindAll(room => room.roomName.Contains("Easy"));
            return easyRooms[Random.Range(0, easyRooms.Count)].roomName;
        }

        if (_iterations < numRooms/3 * 2)
        {
            // Return a medium room
            var mediumRooms = _roomData.FindAll(room => room.roomName.Contains("Medium"));
            return mediumRooms[Random.Range(0, mediumRooms.Count)].roomName;
        }

        if (_iterations > numRooms) return null;
        {
            // Determine if the trader should be spawned
            var trader = _roomData.Find(room => room.roomName == "Trader");
            if (trader != null)
            {
                // The probability of spawning it is iterations / numRooms chance
                if (Random.Range(0, numRooms) < _iterations)
                {
                    _roomData.Remove(trader);
                    return trader.roomName;
                }
                
                // If this is the last room, and the trader hasn't spawned, spawn it
                if (_iterations == numRooms && _roomData.Contains(trader))
                {
                    _roomData.Remove(trader);
                    return trader.roomName;
                }
            }
            
            // Return a hard room
            var hardRooms = _roomData.FindAll(room => room.roomName.Contains("Hard"));
            return hardRooms[Random.Range(0, hardRooms.Count)].roomName;
        }

    }
}
