using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class RoomInfo
{
    public string Name;
    public int X;
    public int Y;
}

public class RoomController : MonoBehaviour
{
    #region Roomcontroller references

    public static RoomController Instance;
    private RoomInfo _currentLoadRoomData;
    public TextMeshProUGUI essenceText;

    // Make a queue of rooms to load
    private readonly Queue<RoomInfo> _loadRoomQueue = new();
    public List<Room> loadedRooms = new();

    #endregion
    #region RoomController Variables

    // Make a bool to check if a room is loading
    private bool _isLoadingRoom;
    private bool _spawnedBossRoom;
    public bool updatedRooms;
    public bool generateDungeon = true;

    #endregion
    #region RoomController Events

    // Observer pattern
    public static event Action<Room> OnRoomChange;
    public static event Action<Room> OnRoomCleared;
    public static event Action<Room> OnLoad;

    #endregion

    private void Awake()
    {
        Instance = this;

        Health.OnPlayerDeath += Health_OnPlayerDeath;
    }

    private void Start()
    {
        if (!generateDungeon) return;
        // Apply any active perma seeds
        var activePermaSeeds = PlayerController.Instance.GetActiveSeeds();

        if (activePermaSeeds == null) return;

        // Apply the seed if it is grown
        foreach (var seed in activePermaSeeds.Where(seed => seed.IsGrown()))
        {
            seed.Apply();
        }
    }

    // Unsubscribe on destroy
    private void OnDestroy()
    {
        Health.OnPlayerDeath -= Health_OnPlayerDeath;
    }

    private void Health_OnPlayerDeath()
    {
        // Clear the loaded rooms
        loadedRooms.Clear();

        // Reset everything
        _isLoadingRoom = false;
        _spawnedBossRoom = false;
        updatedRooms = false;
    }

    #region Rooms and Dungeon Generation
    
    private void Update()
    {
        // Load the next room in the queue
        if (_isLoadingRoom || !generateDungeon) return;
        // If there are no rooms in the queue, spawn the boss room
        if (_loadRoomQueue.Count == 0)
        {
            if (!_spawnedBossRoom)
            {
                StartCoroutine(SpawnBossRoom());
            }
            else if (_spawnedBossRoom && !updatedRooms)
            {
                // Remove all unconnected doors from the rooms
                foreach (var room in loadedRooms)
                {
                    room.RemoveUnconnectedDoors();
                }

                updatedRooms = true;

                OnLoad?.Invoke(loadedRooms[0]);
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
            MinimapManager.Instance.LoadMinimapRoom("End", room.x, room.y);
            yield break;
        }
    }

    // If the room does not exist, populate it with the required information and
    // add it to the list of loaded rooms
    public void LoadRoom(string roomName, int x, int y)
    {
        if (DoesRoomExist(x, y)) return;

        var newRoomData = new RoomInfo
        {
            Name = roomName,
            X = x,
            Y = y
        };

        _loadRoomQueue.Enqueue(newRoomData);
    }

    public void RegisterRoom(Room room)
    {
        if (!generateDungeon) return;
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
            _currentLoadRoomData.X * room.width,
            _currentLoadRoomData.Y * room.height,
            0
        );

        room.x = _currentLoadRoomData.X;
        room.y = _currentLoadRoomData.Y;
        room.name = GameManager.Instance.currentWorldName + "-" + _currentLoadRoomData.Name + " " +
                    _currentLoadRoomData.X + "," + _currentLoadRoomData.Y;
        transform1.parent = transform;

        _isLoadingRoom = false;

        if (loadedRooms.Count == 0)
        {
            CameraController.Instance.currentRoom = room;
            MiniCameraController.Instance.currentRoom = room;
        }

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


    #endregion

    public void OnPlayerEnterRoom(Room room)
    {
        // Update minimap icons
        var prevRoom = CameraController.Instance.currentRoom;

        // Disable all the adjacent room icons
        if (prevRoom != null && MinimapManager.Instance != null)
        {
            // get the corresponding minimap icon
            var minimapRoom = MinimapManager.Instance.FindRoom(prevRoom.x, prevRoom.y);
            if (minimapRoom != null)
            {
                // Disable the adjacent room icons
                foreach (var adjRoom in minimapRoom.connectedRooms)
                {
                    adjRoom.DisableIfNotVisited();
                }
            }
        }

        // Update the camera
        CameraController.Instance.currentRoom = room;
        if (MiniCameraController.Instance != null)
        {
            MiniCameraController.Instance.currentRoom = room;
        }

        // Invoke the OnPlayerEnterRoom event to lock the doors of the room
        OnRoomChange?.Invoke(room);
    }

    public void InvokeOnRoomChange()
    {
        OnRoomChange?.Invoke(GameManager.Instance.activeRoom);
    }

    public void OnPlayerClearRoom(Room room)
    {
        // Get the corresponding minimap room
        if (MinimapManager.Instance != null)
        {
            var minimapRoom = MinimapManager.Instance.FindRoom(room.x, room.y);
            if (minimapRoom != null)
            {
                minimapRoom.SetPurified();
            }
        }

        // Invoke the OnPlayerClearRoom event to unlock the doors of the room
        OnRoomCleared?.Invoke(room);
    }
}