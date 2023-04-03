using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Room activeRoom;
    // Make a list of prefabs for the room types
    public List<GameObject> roomPrefabs = new ();
    public string currentWorldName = "Forest";

    public static event Action<Room> OnStartWave;

    private void Awake()
    {
        Instance = this;
        
        // Subscribe to the OnRoomChange event
        RoomController.OnRoomChange += RoomController_OnRoomChange;
        // Subscribe to the OnRoomCleared event
        RoomController.OnRoomCleared += RoomController_OnRoomCleared;
    }
    
    public GameObject GetRoomPrefab(string roomType)
    {
        // Find the room prefab with the same name as the room type
        var roomPrefab = roomPrefabs.Find(prefab => prefab.name == currentWorldName + roomType);
        return roomPrefab;
    }
    
    private void RoomController_OnRoomChange(Room room)
    {
        // Set the active room to the new room
        activeRoom = room;
        
        // If the room has a tag of EnemyRoom, lock the doors
        if (!room.CompareTag("EnemyRoom")) return;
        foreach (var door in room.doors)
        {
            door.LockDoor();    
        }
        
        // Spawn an enemy in the current room
        OnStartWave?.Invoke(room);
    }
    
    private void RoomController_OnRoomCleared(Room room)
    {
        // If the room has a tag of EnemyRoom, unlock the doors
        if (!room.CompareTag("EnemyRoom")) return;
        foreach (var door in room.doors)
        {
            door.UnlockDoor();    
        }
    }
}
