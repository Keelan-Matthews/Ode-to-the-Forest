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
        // Set this room to visited
        minimapRoom.SetVisited();

        // Enable the adjacent rooms
        var adjacentRooms = minimapRoom.connectedRooms;
        
        foreach (var adjacentRoom in adjacentRooms)
        {
            adjacentRoom.spriteRenderer.enabled = true;
        }
    }

    private void Update()
    {
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
