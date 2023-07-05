using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MinimapManager : MonoBehaviour
{
    private readonly Queue<RoomInfo> _minimapRooms = new ();
    private RoomInfo _currentLoadRoomData;
    private bool _isLoadingRoom;
    private bool _updatedRooms;
    public List<MinimapRoom> loadedRooms = new ();
    public bool showMinimap;
    public GameObject minimapTexture;
    
    public static MinimapManager Instance;
    
    private void Awake()
    {
        Instance = this;
        
        RoomController.OnRoomChange += UpdateIcon;
        RoomController.OnLoad += UpdateIcon;
    }

    private void UpdateIcon(Room obj)
    {
        // Get the corresponding minimap room
        var minimapRoom = FindRoom(obj.x, obj.y);
        // Return if the room is null
        if (minimapRoom == null) return;
        
        // Enable this room
        minimapRoom.spriteRenderer.enabled = true;
        minimapRoom.iconRenderer.enabled = true;
        // Set this room to visited 
        minimapRoom.SetVisited();
        
        // If the player is currently in the room, set the icon to "Active", else set it to,
        // the rooms original icon
        if (obj.x == GameManager.Instance.activeRoom.x && obj.y == GameManager.Instance.activeRoom.y)
        {
            minimapRoom.SetRoomIcon("Active");
        }

        // Enable the adjacent rooms
        var adjacentRooms = minimapRoom.connectedRooms;
        
        foreach (var adjacentRoom in adjacentRooms)
        {
            adjacentRoom.spriteRenderer.enabled = true;
            
            // If an adjacent room name contains "End", set the icon to the boss icon prematurely
            if (adjacentRoom.name.Contains("End"))
            {
                adjacentRoom.iconRenderer.enabled = true;
            }
            
            // If the adjacent room is visited, set it back to its original icon
            if (adjacentRoom.visited)
            {
                // Extract "Start" from the component name Minimap-Start 0,0
                var roomType = adjacentRoom.name.Split('-')[1];
                // remove the coordinates from the room type
                roomType = roomType.Split(' ')[0];
                adjacentRoom.SetRoomIcon(roomType);
            }
        }
    }

    private void Update()
    {
        // Show or hide the minimap based on the showMinimap bool
        minimapTexture.SetActive(showMinimap);
        
        // If all the rooms are loaded, determine their doors
        if (!_updatedRooms && RoomController.Instance.updatedRooms)
        {
            // For each room in the loaded rooms
            foreach (var room in loadedRooms)
            {
                room.DetermineDoors();
            }
            
            _updatedRooms = true;
            
            // Update the minimap
            UpdateIcon(GameManager.Instance.activeRoom);
        }

        // Return if the room is loading or the queue is empty
        if (_isLoadingRoom || _minimapRooms.Count == 0) return;

        _currentLoadRoomData = _minimapRooms.Dequeue();
        _isLoadingRoom = true;
        
        // Add the current room prefab to the scene at the specified position
        var roomPrefab = GameManager.Instance.GetMinimapRoomPrefab(_currentLoadRoomData.Name);
        Instantiate(roomPrefab, new Vector3(_currentLoadRoomData.X, _currentLoadRoomData.Y, 0), Quaternion.identity);
    }

    public void LoadMinimapRoom(string roomName, int x, int y)
    {
        if (DoesRoomExist(x, y)) return;

        var newRoomData = new RoomInfo
        {
            Name = roomName,
            X = x,
            Y = y
        };

        _minimapRooms.Enqueue(newRoomData);
    }
    
    public bool DoesRoomExist(int x, int y)
    {
        return loadedRooms.Find(r => r.x == x && r.y == y) != null;
    }
    
    public void RegisterMinimapRoom(MinimapRoom room)
    {
        // If the room already exists, destroy it
        if (DoesRoomExist(_currentLoadRoomData.X, _currentLoadRoomData.Y))
        {
            Destroy(room.gameObject);
            _isLoadingRoom = false;
            return;
        }
        
        // Set the room's position and name
        var transform1 = room.transform;
        transform1.position = new Vector3(
            _currentLoadRoomData.X * room.Width,
            _currentLoadRoomData.Y * room.Height,
            0
        );
        
        room.x = _currentLoadRoomData.X;
        room.y = _currentLoadRoomData.Y;
        room.name = "Minimap-" + _currentLoadRoomData.Name + " " + _currentLoadRoomData.X + "," + _currentLoadRoomData.Y;
        transform1.parent = transform;
        
        _isLoadingRoom = false;

        loadedRooms.Add(room);
    }
    
    public MinimapRoom FindRoom(int x, int y)
    {
        return loadedRooms.Find(r => r.x == x && r.y == y);
    }
}
