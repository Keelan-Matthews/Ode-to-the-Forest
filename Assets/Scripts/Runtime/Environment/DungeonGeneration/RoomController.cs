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
        
        // Get the last room in the list and replace it with the boss room
        var bossRoom = loadedRooms[^1];
        var tempRoom = new Vector2Int(bossRoom.x, bossRoom.y);
        Destroy(bossRoom.gameObject);
        
        var roomToRemove = loadedRooms.Single(r => r.x == tempRoom.x && r.y == tempRoom.y);
        loadedRooms.Remove(roomToRemove);
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
