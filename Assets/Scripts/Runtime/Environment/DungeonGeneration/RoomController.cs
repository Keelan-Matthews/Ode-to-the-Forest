using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Runtime.Abilities;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using Random = System.Random;
using UnityEditor;
using System.Reflection;

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
    public List<GameObject> bushPrefabs;
    public GameObject dialogueComponent;
    [SerializeField] private TextMeshPro deeperPortalTitle;
    [SerializeField] private TextMeshPro deeperPortalText;
    [SerializeField] private AudioSource backgroundMusic;
    private bool _fadedOut;
    public Camera forestCamera;
    private int _bossRoomX;
    private int _bossRoomY;

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
    public string currentSeed;
    public bool hasStructuredRandomGeneration;
    public int generationSeedCount = 10;

    #endregion

    #region RoomController Events

    // Observer pattern
    public static event Action<Room> OnRoomChange;
    public static event Action<Room> OnRoomCleared;
    public static event Action<Room> OnLoad;

    #endregion
    
    private float _backgroundMusicVolume;

    private void Awake()
    {
        Instance = this;

        GameManager.OnContinue += GameManager_OnContinue;

        if (hasStructuredRandomGeneration)
        {
            if (generationSeedCount > GameManager.Instance.TimesEnteredDungeon && GameManager.Instance.generationSeeds.Count > 0)
            {
                var randomIndex = UnityEngine.Random.Range(0, GameManager.Instance.generationSeeds.Count);
                currentSeed = GameManager.Instance.generationSeeds[randomIndex];
                GameManager.Instance.generationSeeds.RemoveAt(randomIndex);
            }
            else
            {
                currentSeed = "";
            }
        }

        if (!currentSeed.Equals(""))
        {
            SetRandomSeed(currentSeed);
        }
        else
        {
            GenerateRandomSeed();
        }
    }

    private void Start()
    {
        GameManager.Instance.TimesEnteredDungeon++;
        if (backgroundMusic != null)
        {
            _backgroundMusicVolume = backgroundMusic.volume;
        }
    }

    public void GenerateRandomSeed()
    {
        var tempSeed = (int) DateTime.Now.Ticks;
        currentSeed = tempSeed.ToString();
        // Set the seed
        UnityEngine.Random.InitState(tempSeed);
        
        Debug.Log("Generated random seed: " + currentSeed);
    }

    public void SetRandomSeed(string seed = "")
    {
        currentSeed = seed;
        int tempSeed = 0;
        
        if (seed == "")
        {
            tempSeed = (int) DateTime.Now.Ticks;
            currentSeed = tempSeed.ToString();
        }
        else
        {
            tempSeed = int.Parse(seed);
            UnityEngine.Random.InitState(tempSeed);
        }
    }
    
    public void EnableDeathPostProcessing()
    {
        PostProcessControls.Instance.SetDeathProfile();
        PostProcessControls.Instance.RampUpWeightCoroutine();
        StartCoroutine(SlowMusic());
    }
    
    private IEnumerator SlowMusic()
    {
        var targetPitch = 0.5f;
        var duration = 1f;
        backgroundMusic.time = 0.3f;
        while (backgroundMusic.pitch > targetPitch)
        {
            backgroundMusic.pitch -= Time.deltaTime / duration;
            yield return null;
        }
    }

    public void DisableDeathPostProcessing()
    {
        PostProcessControls.Instance.ResetWeightCoroutine();
        backgroundMusic.Stop();
    }

    private void OnDungeonFinished()
    {
        // if (!generateDungeon) return;
        // Apply any active perma seeds
        var activePermaSeeds = PermaSeedManager.Instance.GetActiveSeeds();

        if (activePermaSeeds == null) return;

        // Apply the seed if it is grown
        foreach (var seed in activePermaSeeds)
        {
            if (seed == null || !seed.IsGrown()) continue;
            PlayerController.Instance.AddAbility(seed.GetAbilityEffect());

            // If it is not the minimap seed, trigger the ability display
            if (seed.seedName == "Minimap") continue;
            AbilityManager.Instance.TriggerAbilityDisplay(seed.abilityEffect);
        }
        
        if (GameManager.Instance.deeperPortalSpawn && !GameManager.Instance.deeperPortalSpawnPrompted)
        {
            deeperPortalTitle.enabled = true;
            deeperPortalText.enabled = true;
            StartCoroutine(FadeInNewDayText());
            StartCoroutine(FadeOutNewDayText());
            GameManager.Instance.deeperPortalSpawnPrompted = true;
            // Play the wave start sound
            AudioManager.PlaySound(AudioManager.Sound.WaveStart, transform.position);
            // DataPersistenceManager.Instance.SaveGame(true);
        }
    }
    
    private IEnumerator FadeOutMusic()
    {
        var volume = _backgroundMusicVolume;
        var duration = 2f;
        while (volume > 0f)
        {
            volume -= Time.deltaTime / duration;
            backgroundMusic.volume = volume;
            yield return null;
        }
        _fadedOut = true;
    }
    
    private IEnumerator FadeInMusic()
    {
        var volume = 0f;
        var duration = 2f;
        while (volume < _backgroundMusicVolume)
        {
            volume += Time.deltaTime / duration;
            backgroundMusic.volume = volume;
            yield return null;
        }
        _fadedOut = false;
    }
    
    private IEnumerator FadeInNewDayText()
    {
        var alpha = 0f;
        while (alpha < 2f)
        {
            alpha += Time.deltaTime;
            deeperPortalTitle.color = new Color(1f, 1f, 1f, alpha);
            deeperPortalText.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }
    }
    
    private IEnumerator FadeOutNewDayText()
    {
        // Wait 2 seconds and then fade out the text
        yield return new WaitForSeconds(1.5f);

        var alpha = 2f;
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime;
            deeperPortalTitle.color = new Color(1f, 1f, 1f, alpha);
            deeperPortalText.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }
        
        deeperPortalTitle.enabled = false;
        deeperPortalText.enabled = false;
    }

    // Unsubscribe on destroy
    private void OnDestroy()
    {
        GameManager.OnContinue -= GameManager_OnContinue;
    }

    private void GameManager_OnContinue()
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
                    if (room.hasUnconnectedDoorsRemoved) continue; // Check if unconnected doors have been removed
                    room.RemoveUnconnectedDoors();
                    room.hasUnconnectedDoorsRemoved = true;  // Set the flag to true once removal is done
                }

                updatedRooms = true;

                OnLoad?.Invoke(loadedRooms[0]);

                OnDungeonFinished();
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

        // // Find the manhattan distance for each loaded room, and get the room with the highest distance
        // var maxDistance = 0;
        // var tempRoom = new Vector2Int();
        // foreach (var room in loadedRooms)
        // {
        //     var distance = Math.Abs(room.x) + Math.Abs(room.y);
        //     if (distance <= maxDistance) continue;
        //     maxDistance = distance;
        //     tempRoom = new Vector2Int(room.x, room.y);
        // }
        
        // Find a Hard or extreme room that is the furthest away from the start room
        var tempRoom = new Vector2Int();
        var maxDistance = 0;
        var adjRoomCountOfMaxDistanceRoom = 0;
        foreach (var room in loadedRooms)
        {
            var distance = Math.Abs(room.x) + Math.Abs(room.y);
            if (distance < maxDistance) continue;
            if (!room.name.Contains("Hard") && !room.name.Contains("Extreme")) continue;
            
            // If this room has more adjacent rooms than the current max distance room, skip it
            var adjRooms1 = GetAdjacentRooms(room.x, room.y);
            if (adjRooms1.Count < adjRoomCountOfMaxDistanceRoom) continue;
            
            maxDistance = distance;
            adjRoomCountOfMaxDistanceRoom = GetAdjacentRooms(room.x, room.y).Count;
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
            _bossRoomX = tempRoom.x;
            _bossRoomY = tempRoom.y;
            yield break;
        }
        
        
        // If the boss room did not spawn, spawn it in the room with the highest distance
        LoadRoom("End", tempRoom.x, tempRoom.y, true);
        MinimapManager.Instance.LoadMinimapRoom("End", tempRoom.x, tempRoom.y);
        _bossRoomX = tempRoom.x;
        _bossRoomY = tempRoom.y;
    }

    // If the room does not exist, populate it with the required information and
    // add it to the list of loaded rooms
    public void LoadRoom(string roomName, int x, int y, bool replace = false)
    {
        if (DoesRoomExist(x, y) && !replace) return;

        var newRoomData = new RoomInfo
        {
            Name = roomName,
            X = x,
            Y = y
        };
        
        if (replace)
        {
            // Remove the room from the list of loaded rooms
            var roomToRemove = loadedRooms.Find(room => room.x == x && room.y == y);
            // Destroy the room
            Destroy(roomToRemove.gameObject);
            loadedRooms.Remove(roomToRemove);
        }

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
        
        // If it is the boss room, fade out the music
        if (room.name.Contains("End"))
        {
            StartCoroutine(FadeOutMusic());
        }
        else if (_fadedOut)
        {
            StartCoroutine(FadeInMusic());
        }

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

    // This function decreases all the sunlight radii in the dungeon in room's that have not been cleared
    public void DecreaseSunlightRadius()
    {
        foreach (var room in loadedRooms)
        {
            if (room.cleared) continue;
            room.DecreaseSunlight();
        }
    }

    // This function increases all the sunlight radii in the dungeon in room's that have not been cleared
    public void IncreaseSunlightRadius()
    {
        foreach (var room in loadedRooms)
        {
            if (room.cleared) continue;
            room.IncreaseSunlight();
        }
    }
}