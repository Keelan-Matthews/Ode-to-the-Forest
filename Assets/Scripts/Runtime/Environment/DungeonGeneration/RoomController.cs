using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomInfo
{
    public string Name;
    public int X;
    public int Y;
}

public class RoomController : MonoBehaviour
{
    public static RoomController Instance;
    private RoomInfo _currentLoadRoomData;
    public Room currRoom;
    Queue<RoomInfo> _loadRoomQueue = new ();
    public List<Room> loadedRooms = new ();
    private bool _isLoadingRoom = false;
    private bool _spawnedBossRoom = false;
    private bool _updatedRooms = false;
    
    public static event Action<Room> OnRoomChange;
    public static event Action<Room> OnRoomCleared;


    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        // Load the next room in the queue
        if (_isLoadingRoom) return;
        // If there are no rooms in the queue, spawn the boss room
        if (_loadRoomQueue.Count == 0)
        {
            if (!_spawnedBossRoom)
            {
                StartCoroutine(SpawnBossRoom());
            } 
            else if (_spawnedBossRoom && !_updatedRooms)
            {
                // Remove all unconnected doors from the rooms
                foreach (var room in loadedRooms)
                {
                    room.RemoveUnconnectedDoors();
                }
                _updatedRooms = true;
            }
            
            return;
        }
        
        _currentLoadRoomData = _loadRoomQueue.Dequeue();
        _isLoadingRoom = true;
        
        // Add the current room prefab to the scene at the specified position
        var roomPrefab = GameManager.Instance.GetRoomPrefab(_currentLoadRoomData.Name);
        Instantiate(roomPrefab, new Vector3(_currentLoadRoomData.X, _currentLoadRoomData.Y, 0), Quaternion.identity);
    }
    
    private IEnumerator SpawnBossRoom()
    {
        _spawnedBossRoom = true;
        yield return new WaitForSeconds(0.5f);
        if (_loadRoomQueue.Count != 0) yield break;

        // Find the manhattan distance for each loaded room, and get the room with the highest distance
        var maxDistance = 0;
        var tempRoom = new Vector2Int();
        foreach (var room in loadedRooms)
        {
            var distance = Math.Abs(room.x) + Math.Abs(room.y);
            if (distance <= maxDistance) continue;
            maxDistance = distance;
            tempRoom = new Vector2Int(room.x, room.y);
        }
        
        // Get the adjacent rooms to the room with the highest distance
        var adjRooms = GetAdjacentRooms(tempRoom.x, tempRoom.y);

        // Spawn the boss room in an adjacent room that does not have more than 1 adjacent rooms
        foreach (var room in adjRooms)
        {
            var adjRoomsToAdjRoom = GetAdjacentRooms(room.x, room.y);
            if (adjRoomsToAdjRoom.Count > 1) continue;
            LoadRoom("End", room.x, room.y);
            yield break;
        }
    }

    // If the room does not exist, populate it with the required information and
    // add it to the list of loaded rooms
    public void LoadRoom(string name, int x, int y)
    {
        if (DoesRoomExist(x, y)) return;

        var newRoomData = new RoomInfo();
        newRoomData.Name = name;
        newRoomData.X = x;
        newRoomData.Y = y;
        
        _loadRoomQueue.Enqueue(newRoomData);
    }

    public void RegisterRoom(Room room)
    {
        // If the room already exists, destroy it
        if (DoesRoomExist(_currentLoadRoomData.X, _currentLoadRoomData.Y))
        {
            Destroy(room.gameObject);
            _isLoadingRoom = false;
            return;
        }
        
        // Set the room's position and name
        room.transform.position = new Vector3(
            _currentLoadRoomData.X * room.width,
            _currentLoadRoomData.Y * room.height,
            0
        );
        
        room.x = _currentLoadRoomData.X;
        room.y = _currentLoadRoomData.Y;
        room.name = GameManager.Instance.currentWorldName + "-" + _currentLoadRoomData.Name + " " + _currentLoadRoomData.X + "," + _currentLoadRoomData.Y;
        room.transform.parent = transform;
        
        _isLoadingRoom = false;
        
        if(loadedRooms.Count == 0)
            CameraController.Instance.currentRoom = room;
        
        loadedRooms.Add(room);
    }

    public bool DoesRoomExist(int x, int y)
    {
        return loadedRooms.Find(r => r.x == x && r.y == y) != null;
    }
    
    public List<Vector2Int> GetAdjacentRooms(int x, int y)
    {
        // Find a door without a room connected to it and spawn the boss room there
        var adjRooms = new List<Vector2Int>();

        // Check if the room to the left exists
        if (!DoesRoomExist(x - 1, y))
        {
            adjRooms.Add(new Vector2Int(x - 1, y));
        }
        // Check if the room to the right exists
        else if (!DoesRoomExist(x + 1, y))
        {
            adjRooms.Add(new Vector2Int(x + 1, y));
        }
        // Check if the room above exists
        else if (!DoesRoomExist(x, y + 1))
        {
            adjRooms.Add(new Vector2Int(x, y + 1));
        }
        // Check if the room below exists
        else if (!DoesRoomExist(x, y - 1))
        {
            adjRooms.Add(new Vector2Int(x, y - 1));
        }
        
        return adjRooms;
    }
    
    // Find the room in loadedRooms with the specified coordinates
    public Room FindRoom(int x, int y)
    {
        return loadedRooms.Find(r => r.x == x && r.y == y);
    }

    /**
     * ADD ALL POSSIBLE ROOMS TO SPAWN HERE
     */
    public string GetRandomRoomName()
    {
        // Remember to add the room to the GameManager's room list
        var possibleRooms = new []
        {
            "Easy",
            "Medium",
            "Hard"
        };
        
        return possibleRooms[UnityEngine.Random.Range(0, possibleRooms.Length)];
    }
    
    public void OnPlayerEnterRoom(Room room)
    {
        // Update the camera
        CameraController.Instance.currentRoom = room;
        
        // Invoke the OnPlayerEnterRoom event to lock the doors of the room
        OnRoomChange?.Invoke(room);
    }
    
    public void OnPlayerClearRoom(Room room)
    {
        // Invoke the OnPlayerClearRoom event to unlock the doors of the room
        OnRoomCleared?.Invoke(room);
    }
}
