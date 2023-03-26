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
    string _currentWorldName = "Forest";
    private RoomInfo _currentLoadRoomData;
    public Room currRoom;
    Queue<RoomInfo> _loadRoomQueue = new ();
    public List<Room> loadedRooms = new ();
    private bool _isLoadingRoom = false;
    private bool _spawnedBossRoom = false;
    private bool _updatedRooms = false;
    

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
        StartCoroutine(LoadRoomRoutine(_currentLoadRoomData));
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
        
        // Find a door without a room connected to it and spawn the boss room there
        var roomToSpawn = new List<Vector2Int>();

        // Check if the room to the left is loaded
        if (!DoesRoomExist(tempRoom.x - 1, tempRoom.y))
        {
            roomToSpawn.Add(new Vector2Int(tempRoom.x - 1, tempRoom.y));
        }
        // Check if the room to the right is loaded
        else if (!DoesRoomExist(tempRoom.x + 1, tempRoom.y))
        {
            roomToSpawn.Add(new Vector2Int(tempRoom.x + 1, tempRoom.y));
        }
        // Check if the room above is loaded
        else if (!DoesRoomExist(tempRoom.x, tempRoom.y + 1))
        {
            roomToSpawn.Add(new Vector2Int(tempRoom.x, tempRoom.y + 1));
        }
        // Check if the room below is loaded
        else if (!DoesRoomExist(tempRoom.x, tempRoom.y - 1))
        {
            roomToSpawn.Add(new Vector2Int(tempRoom.x, tempRoom.y - 1));
        }
        
        // Find the manhattan distance for each room in roomToSpawn, and get the room with the highest distance
        maxDistance = 0;
        foreach (var room in roomToSpawn)
        {
            var distance = Math.Abs(room.x) + Math.Abs(room.y);
            if (distance <= maxDistance) continue;
            maxDistance = distance;
            tempRoom = new Vector2Int(room.x, room.y);
        }
        
        // Spawn the boss room
        LoadRoom("End", tempRoom.x, tempRoom.y);
    }

    public void LoadRoom(string name, int x, int y)
    {
        if (DoesRoomExist(x, y)) return;
        
        var newRoomData = new RoomInfo();
        newRoomData.Name = name;
        newRoomData.X = x;
        newRoomData.Y = y;
        
        _loadRoomQueue.Enqueue(newRoomData);
    }

    private IEnumerator LoadRoomRoutine(RoomInfo info)
    {
        var roomName = _currentWorldName + info.Name;
        var loadRoom = SceneManager.LoadSceneAsync(roomName, LoadSceneMode.Additive);
        
        while(loadRoom.isDone == false)
        {
            yield return null;
        }
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
        
        room.transform.position = new Vector3(
            _currentLoadRoomData.X * room.width,
            _currentLoadRoomData.Y * room.height,
            0
        );
        
        room.x = _currentLoadRoomData.X;
        room.y = _currentLoadRoomData.Y;
        room.name = _currentWorldName + "-" + _currentLoadRoomData.Name + " " + _currentLoadRoomData.X + "," + _currentLoadRoomData.Y;
        room.transform.parent = transform;
        
        _isLoadingRoom = false;
        
        if(loadedRooms.Count == 0)
            CameraController.Instance.currentRoom = room;
        
        loadedRooms.Add(room);
        room.RemoveUnconnectedDoors();
    }

    public bool DoesRoomExist(int x, int y)
    {
        return loadedRooms.Find(r => r.x == x && r.y == y) != null;
    }
    
    public Room FindRoom(int x, int y)
    {
        return loadedRooms.Find(r => r.x == x && r.y == y);
    }
    
    public void OnPlayerEnterRoom(Room room)
    {
        CameraController.Instance.currentRoom = room;
        currRoom = room;
    }
}
