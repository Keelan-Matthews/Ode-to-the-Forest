using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Room : MonoBehaviour
{
    #region Room Dimensions

    public int width;
    public int height;
    public int x;
    public int y;
    private const float WallOffset = 4f;

    #endregion

    #region Room references

    public List<Door> doors = new();
    public List<Room> connectedRooms = new();
    public GameObject roomBackground;
    private SunlightController _sunlightController;
    [SerializeField] private GameObject sunlightDustParticles;
    [SerializeField] private SporadicSunlightController sporadicSunlightController;
    [SerializeField] private ParticleSystem purifyParticles;

    [Serializable]
    public struct EnemySpawnerData
    {
        public string name;
        public SpawnerData spawnerData;
    }

    public List<EnemySpawnerData> enemySpawners = new();
    private static readonly int IsPurified = Animator.StringToHash("IsPurified");

    #endregion

    #region Room Variables

    private bool _updatedDoors;
    public bool hasWave = true;
    public bool cleared;
    public bool spawnedPermaSeed;
    public bool hasDialogue;
    public bool hasUnconnectedDoorsRemoved;
    private bool _isPurifying;
    [SerializeField] private bool showName;
    [SerializeField] private TextMeshPro roomNameText;
    private bool exitedRoom;
    private bool _startedRemovingDoors;

    #endregion

    #region Room Difficulty

    private int _difficulty;
    public float waveStartTime;
    public float timeInDarkness = 3.5f;
    public float waveDuration = 10f;

    #endregion

    // Start is called before the first frame update
    private void Start()
    {
        _sunlightController = GetComponentInChildren<SunlightController>();

        if (RoomController.Instance == null)
        {
            throw new Exception("Scene not found");
        }

        // Add the doors in the room to the list and set the door variables
        var roomDoors = GetComponentsInChildren<Door>();
        foreach (var door in roomDoors)
        {
            doors.Add(door);
        }

        RoomController.Instance.RegisterRoom(this);

        // Set the difficulty of the room
        SetDifficulty();
    }

    public void DecreaseSunlight()
    {
        _sunlightController.DecreaseRadius();
    }

    public void IncreaseSunlight()
    {
        _sunlightController.IncreaseRadius();
    }

    private void Update()
    {
        // If we haven't updated the doors yet and the room is the end room, update the doors
        if (name.Contains("End") && !_updatedDoors && !_startedRemovingDoors)
        {
            _startedRemovingDoors = true;
            RemoveUnconnectedDoors();
            _updatedDoors = true;
        }
    }

    private void SetDoorType(string roomName, Door.DoorType doorType)
    {
        // If the roomName contains "VendingMachine", set the door type to vending machine
        // the doorType tells the function which door in the room to set
        if (roomName.Contains("VendingMachine"))
        {
            foreach (var door in doors.Where(door => door.doorType == doorType))
            {
                door.SetDoorType("Obelisk");
            }
        }
        else if (roomName.Contains("Trader"))
        {
            foreach (var door in doors.Where(door => door.doorType == doorType))
            {
                door.SetDoorType("Trader");
            }
        }
        else if (roomName.Contains("Portal"))
        {
            foreach (var door in doors.Where(door => door.doorType == doorType))
            {
                door.SetDoorType("Portal");
            }
        }
        else if (roomName.Contains("End"))
        {
            foreach (var door in doors.Where(door => door.doorType == doorType))
            {
                door.SetDoorType("Boss");
            }
        }
        else if (roomName.Contains("ShrineOfYouth"))
        {
            foreach (var door in doors.Where(door => door.doorType == doorType))
            {
                door.SetDoorType("Shrine");
            }
        }
        else if (roomName.Contains("Collector"))
        {
            foreach (var door in doors.Where(door => door.doorType == doorType))
            {
                door.SetDoorType("Collector");
            }
        }
    }

    // Set the difficulty of the room
    private void SetDifficulty()
    {
        // If name does not contain "Forest", or it is a portal, vending machine or trader room, return
        if (!name.Contains("Forest") || name.Contains("Portal") || name.Contains("VendingMachine") ||
            name.Contains("Trader")) return;

        // Get the difficulty from the room name (ForestEasy = Easy)
        // Split the room name into "Forest" and whatever is after it
        var splitName = name.Split(new[] { "Forest-" }, StringSplitOptions.None);
        if (splitName.Length < 2) return;
        var d = splitName[1].Split(' ')[0];
        switch (d)
        {
            case "Easy":
                _difficulty = 0;
                waveDuration = 10f;
                break;
            case "Medium":
                _difficulty = 1;
                waveDuration = 15f;
                break;
            case "Hard":
                _difficulty = 2;
                waveDuration = 20f;
                break;
            case "Extreme":
                _difficulty = 2;
                waveDuration = 20f;
                break;
            default:
                break;
        }
    }

    public int GetDifficulty()
    {
        return _difficulty;
    }

    #region Room Initialisation & Getters

    public void RemoveUnconnectedDoors()
    {
        var isBossRoom = name.Contains("End");

        foreach (var door in doors)
        {
            var adjacentRoom = GetAdjacentRoom(door.doorType);

            if (adjacentRoom == null)
            {
                door.isActive = false;
                door.gameObject.SetActive(false);
            }
            else
            {
                var shouldHideDoor = ShouldHideDoor(adjacentRoom, isBossRoom);
                if (door.hasBeenReverted)
                {
                    shouldHideDoor = false;
                }
                
                door.gameObject.SetActive(!shouldHideDoor);
                door.isActive = !shouldHideDoor;

                if (shouldHideDoor)
                {
                    // Get the adjacent room's door that connects to this room
                    var adjacentRoomDoor = GetOppositeDoor(door.doorType);
                    adjacentRoom.HideDoor(adjacentRoomDoor);
                }
                
                // // If, after hiding the door, you cannot get to the boss room, unhide the door
                if (!name.Contains("End") && !name.Contains("Hard") && !name.Contains("Extreme") && !name.Contains("Start"))
                {
                    if (shouldHideDoor && !CanGetToBossRoom())
                    {
                        UnhideDoor(door.doorType, adjacentRoom);
                        var adjacentRoomDoor = GetOppositeDoor(door.doorType);
                        adjacentRoom.UnhideDoor(adjacentRoomDoor, this);
                        Debug.Log($"Can't get to boss room, reverting room {name} door {door.doorType} to normal");
                    }
                }

                if (shouldHideDoor) continue;
                connectedRooms.Add(adjacentRoom);
                // Only set the door type if it is not a boss room
                if (!isBossRoom)
                {
                    SetDoorType(adjacentRoom.name, door.doorType);
                }
            }
        }
        

    }
    
    private void HideDoor(Door.DoorType doorType)
    {
        var door = doors.Find(d => d.doorType == doorType);
        door.gameObject.SetActive(false);
        door.isActive = false;
    }
    
    private void UnhideDoor(Door.DoorType doorType, Room adjRoom)
    {
        var door = doors.Find(d => d.doorType == doorType);
        door.gameObject.SetActive(true);
        door.isActive = true;
        door.hasBeenReverted = true;
        
        // Re set the door type
        SetDoorType(adjRoom.name, doorType);
    }

    public bool ShouldHideDoor(Room adjacentRoom, bool isBossRoom)
    {
        if (adjacentRoom == null)
        {
            return true;
        }
        
        // If this room and the adjacent room are ONLY connected by a single door, don't hide it
        if (adjacentRoom.connectedRooms.Count == 1 && connectedRooms.Count == 1)
        {
            return false;
        }

        // If this is the boss room and the adjacent room is Easy or Start, hide the door
        // If this is an Easy or Start room and the adjacent room is the boss room, hide the door
        if (isBossRoom && (adjacentRoom.name.Contains("Easy") || adjacentRoom.name.Contains("Start")))
        {
            return true;
        }
        
        if ((name.Contains("Easy") || name.Contains("Start")) && adjacentRoom.name.Contains("End"))
        {
            return true;
        }
        
        // If this is the boss room and the adjacent room is a VendingMachine room, trader room or Medium room, hide the door
        // If this is a VendingMachine room, trader room or Medium room and the adjacent room is the boss room, hide the door
        if (isBossRoom && (adjacentRoom.name.Contains("VendingMachine") || adjacentRoom.name.Contains("Trader") || adjacentRoom.name.Contains("Medium")))
        {
            return true;
        }
        
        if ((name.Contains("VendingMachine") || name.Contains("Trader") || name.Contains("Medium")) && adjacentRoom.name.Contains("End"))
        {
            return true;
        }

        // If this is an Easy or Start room and the adjacent room is a Hard or Extreme room, hide the door
        // If this is a Hard or Extreme room and the adjacent room is an Easy or Start room, hide the door
        if ((name.Contains("Easy") || name.Contains("Start")) &&
            (adjacentRoom.name.Contains("Hard") || adjacentRoom.name.Contains("Extreme")))
        {
            return true;
        }

        if ((name.Contains("Hard") || name.Contains("Extreme")) &&
            (adjacentRoom.name.Contains("Easy") || adjacentRoom.name.Contains("Start")))
        {
            return true;
        }

        // If this is a VendingMachine room and the adjacent room is a Hard or Extreme room, hide the door
        // If this is a Hard or Extreme room and the adjacent room is a VendingMachine room, hide the door
        if (name.Contains("VendingMachine") &&
            (adjacentRoom.name.Contains("Hard") || adjacentRoom.name.Contains("Extreme")))
        {
            return true;
        }

        if ((name.Contains("Hard") || name.Contains("Extreme")) && adjacentRoom.name.Contains("VendingMachine"))
        {
            return true;
        }

        // If this is the Trader room and the adjacent room is a Hard or Extreme room, hide the door
        // If this is a Hard or Extreme room and the adjacent room is the Trader room, hide the door
        if (name.Contains("Trader") && (adjacentRoom.name.Contains("Hard") || adjacentRoom.name.Contains("Extreme")))
        {
            return true;
        }

        if ((name.Contains("Hard") || name.Contains("Extreme")) && adjacentRoom.name.Contains("Trader"))
        {
            return true;
        }

        // If this is the collector room and the adjacent room is an Easy or Start room, hide the door
        // If this is an Easy or Start room and the adjacent room is the collector room, hide the door
        if (name.Contains("Collector") && (adjacentRoom.name.Contains("Easy") || adjacentRoom.name.Contains("Start")))
        {
            return true;
        }

        if ((name.Contains("Easy") || name.Contains("Start")) && adjacentRoom.name.Contains("Collector"))
        {
            return true;
        }

        // If this room is Shrine of Youth and the adjacent room is an Easy or Start room, hide the door
        // If this is an Easy or Start room and the adjacent room is Shrine of Youth, hide the door
        if (name.Contains("ShrineOfYouth") &&
            (adjacentRoom.name.Contains("Easy") || adjacentRoom.name.Contains("Start")))
        {
            return true;
        }

        if ((name.Contains("Easy") || name.Contains("Start")) && adjacentRoom.name.Contains("ShrineOfYouth"))
        {
            return true;
        }

        if (name.Contains("Portal") && (adjacentRoom.name.Contains("Easy") || adjacentRoom.name.Contains("Start")))
        {
            return true;
        }

        if ((name.Contains("Easy") || name.Contains("Start")) && adjacentRoom.name.Contains("Portal"))
        {
            return true;
        }
        
        // If this is the trader room and the adjacent room is the collector or shrine of youth room, hide the door
        // If this is the collector or shrine of youth room and the adjacent room is the trader room, hide the door
        if (name.Contains("Trader") && (adjacentRoom.name.Contains("Collector") || adjacentRoom.name.Contains("ShrineOfYouth")))
        {
            return true;
        }
        
        if ((name.Contains("Collector") || name.Contains("ShrineOfYouth")) && adjacentRoom.name.Contains("Trader"))
        {
            return true;
        }
        
        // If this is the VendingMachine room and the adjacent room is the collector or shrine of youth room, hide the door
        // If this is the collector or shrine of youth room and the adjacent room is the VendingMachine room, hide the door
        if (name.Contains("VendingMachine") && (adjacentRoom.name.Contains("Collector") || adjacentRoom.name.Contains("ShrineOfYouth")))
        {
            return true;
        }
        
        if ((name.Contains("Collector") || name.Contains("ShrineOfYouth")) && adjacentRoom.name.Contains("VendingMachine"))
        {
            return true;
        }

        return false;
    }

    private Door.DoorType GetOppositeDoor(Door.DoorType type)
    {
        if (type == Door.DoorType.Top)
        {
            return Door.DoorType.Bottom;
        }
        
        if (type == Door.DoorType.Bottom)
        {
            return Door.DoorType.Top;
        }
        
        if (type == Door.DoorType.Left)
        {
            return Door.DoorType.Right;
        }
        
        if (type == Door.DoorType.Right)
        {
            return Door.DoorType.Left;
        }

        return Door.DoorType.Top;
    }
    
    // A recursive function that checks if the player can get to the boss room
    // private bool CanGetToBossRoom(Room room, HashSet<Room> visitedRooms)
    // {
    //     if (room == null || visitedRooms.Contains(room))
    //     {
    //         return false;
    //     }
    //
    //     visitedRooms.Add(room);
    //     
    //     if (room.name.Contains("End"))
    //     {
    //         return true;
    //     }
    //
    //     Door.DoorType[] directions = { Door.DoorType.Right, Door.DoorType.Left, Door.DoorType.Top, Door.DoorType.Bottom };
    //
    //     foreach (var direction in directions)
    //     {
    //         var connectedRoom = room.GetAdjacentRoom(direction);
    //
    //         if (connectedRoom != null && IsDoorActive(direction) && CanGetToBossRoom(connectedRoom, visitedRooms))
    //         {
    //             return true;
    //         }
    //     }
    //
    //     visitedRooms.Remove(room);
    //     return false;
    // }
    //
    // private bool CanGetToBossRoom()
    // {
    //     // Get the start room
    //     var room = RoomController.Instance.FindRoom(0, 0);
    //     return CanGetToBossRoom(room, new HashSet<Room>());
    // }
    
    private bool CanGetToBossRoom()
    {
        var startRoom = RoomController.Instance.FindRoom(0, 0);
        if (startRoom == null)
        {
            return false;
        }

        var visitedRooms = new HashSet<Room>();
        var stack = new Stack<Room>();
        stack.Push(startRoom);

        while (stack.Count > 0)
        {
            var currentRoom = stack.Pop();

            if (currentRoom == null || visitedRooms.Contains(currentRoom))
            {
                continue;
            }

            visitedRooms.Add(currentRoom);

            if (currentRoom.name.Contains("End"))
            {
                return true;
            }

            Door.DoorType[] directions = { Door.DoorType.Right, Door.DoorType.Left, Door.DoorType.Top, Door.DoorType.Bottom };

            foreach (var direction in directions)
            {
                var connectedRoom = currentRoom.GetAdjacentRoom(direction);

                if (connectedRoom != null && IsDoorActive(direction))
                {
                    stack.Push(connectedRoom);
                }
            }
        }

        return false;
    }

    private Room GetAdjacentRoom(Door.DoorType doorType)
    {
        return doorType switch
        {
            Door.DoorType.Right => GetRight(),
            Door.DoorType.Left => GetLeft(),
            Door.DoorType.Top => GetTop(),
            Door.DoorType.Bottom => GetBottom(),
            _ => throw new ArgumentOutOfRangeException(nameof(doorType), doorType, null)
        };
    }

    private bool IsDoorActive(Door.DoorType type)
    {
        foreach (var door in doors)
        {
            if (door.doorType == type)
            {
                return door.isActive;
            }
        }
        
        return false;
    }

    // Get the room to the right of the current room
    public Room GetRight()
    {
        return !RoomController.Instance.DoesRoomExist(x + 1, y) ? null : RoomController.Instance.FindRoom(x + 1, y);
    }

    // Get the room to the left of the current room
    public Room GetLeft()
    {
        return !RoomController.Instance.DoesRoomExist(x - 1, y) ? null : RoomController.Instance.FindRoom(x - 1, y);
    }

    // Get the room above the current room
    public Room GetTop()
    {
        return !RoomController.Instance.DoesRoomExist(x, y + 1) ? null : RoomController.Instance.FindRoom(x, y + 1);
    }

    // Get the room below the current room
    public Room GetBottom()
    {
        return !RoomController.Instance.DoesRoomExist(x, y - 1) ? null : RoomController.Instance.FindRoom(x, y - 1);
    }

    public Vector3 GetRoomCentre()
    {
        return new Vector3(x * width, y * height);
    }

    // Function that gets the spawnable enemies for the room
    public List<EnemySpawnerData> GetEnemyData()
    {
        // Get the list of enemies that can spawn in the room
        return enemySpawners;
    }

    public bool IsCleared()
    {
        return cleared;
    }

    public int GetActiveEnemyCount()
    {
        return GetComponentsInChildren<EnemyController>().Length;
    }

    public GameObject[] GetEnemies()
    {
        return GameObject.FindGameObjectsWithTag("Enemy");
    }

    #endregion

    public void LockRoom()
    {
        foreach (var door in doors)
        {
            door.LockDoor();
        }
    }

    public void UnlockRoom()
    {
        foreach (var door in doors)
        {
            door.UnlockDoor();
        }
    }

    // Set the camera to the current room when the player enters the room
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            exitedRoom = false;
            RoomController.Instance.OnPlayerEnterRoom(this);

            if (showName)
            {
                roomNameText.enabled = true;

                // If the name of the room contains "Trader", play the trader music
                if (name.Contains("Trader"))
                {
                    AudioManager.PlaySound(AudioManager.Sound.EnterTrader, transform.position);
                }
                else if (name.Contains("VendingMachine"))
                {
                    AudioManager.PlaySound(AudioManager.Sound.EnterObelisk, transform.position);
                }
                else if (name.Contains("Portal"))
                {
                    AudioManager.PlaySound(AudioManager.Sound.EnterPortal, transform.position);
                }
                else if (name.Contains("ShrineOfYouth"))
                {
                    AudioManager.PlaySound(AudioManager.Sound.EnterShrineOfYouth, transform.position);
                }
                else if (name.Contains("Collector"))
                {
                    AudioManager.PlaySound(AudioManager.Sound.EnterCollector, transform.position);
                }

                StartCoroutine(FadeInNewDayText());
                StartCoroutine(FadeOutNewDayText());
            }
        }
    }

    private IEnumerator FadeInNewDayText()
    {
        var alpha = 0f;
        while (alpha < 1f)
        {
            alpha += Time.deltaTime;
            roomNameText.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }
    }

    private IEnumerator FadeOutNewDayText()
    {
        // Wait 2 seconds and then fade out the text
        yield return new WaitForSeconds(0.7f);

        var alpha = 2f;
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime;
            roomNameText.color = new Color(1f, 1f, 1f, alpha);

            // If the player has exited the room, stop fading out the text
            if (exitedRoom)
            {
                break;
            }

            yield return null;
        }

        roomNameText.enabled = false;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // showName = false;
            exitedRoom = true;
        }

        if (other.CompareTag("Cloud"))
        {
            Destroy(other.gameObject);
        }
    }

    #region Room Spawning

    public Vector2 GetRandomPositionInRoom(float offset = 0)
    {
        // Randomly pick which axis will be constrained to a side
        var randomAxis = UnityEngine.Random.Range(0, 2);
        var randomPosition = new Vector2();

        switch (randomAxis)
        {
            // If the random axis is 0, constrain the x axis to a side - either left or right
            case 0:
            {
                var randomX = UnityEngine.Random.Range(0, 2);
                // Constrain the axis to a side and either add or subtract the wall offset
                randomPosition.x = randomX == 0 ? -width / 2 + WallOffset + offset : width / 2 - WallOffset - offset;

                // Randomly pick a y position within the height of the room and add or subtract the wall offset
                randomPosition.y =
                    UnityEngine.Random.Range(-height / 2 + WallOffset + offset, height / 2 - WallOffset - offset);
                break;
            }
            // If the random axis is 1, constrain the y axis to a side - either top or bottom
            case 1:
            {
                var randomY = UnityEngine.Random.Range(0, 2);
                randomPosition.y = randomY == 0 ? -height / 2 + WallOffset + offset : height / 2 - WallOffset - offset;

                // Randomly pick a x position within the width of the room
                randomPosition.x =
                    UnityEngine.Random.Range(-width / 2 + WallOffset + offset, width / 2 - WallOffset - offset);
                break;
            }
        }

        // Return the random position and offset it by the room's position
        return randomPosition + (Vector2)transform.position;
    }

    public Vector2 GetRandomPositionInLeftHalfOfRoom(float offset = 0)
    {
        // Calculate the range for the x-axis within the left half of the room
        var minX = -width / 2 + WallOffset + offset;
        var maxX = 0 - WallOffset - offset; // Limit to the center

        // Randomly pick a position within the left half of the room for the x-axis
        var randomX = UnityEngine.Random.Range(minX, maxX);

        // Randomly pick a position within the height of the room for the y-axis
        var randomY = UnityEngine.Random.Range(-height / 2 + WallOffset + offset, height / 2 - WallOffset - offset);

        // Create and return the random position
        var randomPosition = new Vector2(randomX, randomY);

        // Offset the position by the room's position
        return randomPosition + (Vector2)transform.position;
    }

    public static bool ReviewRoomPosition(Vector2 randomPos, Collider2D enemyCollider, bool hasLeeway = true)
    {
        // Determine if the position in the room has enough space to spawn the enemy,
        // but only checking if it collides with Obstacle,Player and Wall tag and not the room collider

        // Calculate the expanded hit radius by adding the leeway to the enemy's bounds
        var leeway = 0f;
        
        if (hasLeeway)
        {
            leeway = 1.2f;
        }
        
        var expandedHitRadius = enemyCollider.bounds.extents.x + leeway;

        var hit = Physics2D.OverlapCircle(randomPos, expandedHitRadius,
            LayerMask.GetMask("Obstacle", "Player", "Wall"));
        return hit == null;
    }

    #endregion

    public void OnWaveEnd(bool bossRoom = false)
    {
        if (cleared) return;
        cleared = true;
        // Play the wave end sound
        AudioManager.PlaySound(AudioManager.Sound.WaveEnd, transform.position);
        if (purifyParticles)
        {
            purifyParticles.Play();
        }

        RoomController.Instance.OnPlayerClearRoom(this);

        if (hasWave && sporadicSunlightController == null)
        {
            _sunlightController.Expand(true);
        }
        else if (sporadicSunlightController != null)
        {
            sporadicSunlightController.spawn = false;
            sporadicSunlightController.Expand();
        }

        // If the room name contains the word "End", manually brighten the room light
        if (name.Contains("End"))
        {
            _sunlightController.enabled = true;
            _sunlightController.BrightenRoomLight();
        }

        // Destroy the sunlight particles
        Destroy(sunlightDustParticles);
        GrowBackground();
        if (!GameManager.Instance.isTutorial && !bossRoom)
        {
            UnlockRoom();
        }

        PurifyObstaclesInRoom();

        // Set inSunlight to true in the PlayerController script
        PlayerController.Instance.inSunlight = true;

        var shakeDuration = bossRoom ? 3f : 0.1f;
        if (CameraController.Instance == null) return;
        CameraController.Instance.GetComponentInParent<CameraShake>().ShakeCamera(shakeDuration, shakeDuration != 3f);
    }

    public void GrowBackground()
    {
        // Trigger the growth animation on the room background
        var roomAnimator = roomBackground.GetComponent<Animator>();
        roomAnimator.SetBool(IsPurified, true);
    }

    private void PurifyObstaclesInRoom()
    {
        if (_isPurifying) return;
        _isPurifying = true;

        // Get the object room spawner in the room
        var roomSpawner = GetComponentInChildren<ObjectRoomSpawner>();
        if (!roomSpawner) return;
        // Get all the obstacles in the room
        var obstacles = roomSpawner.GetObstacles();

        // Play the bush rustle sound
        AudioManager.PlaySound(AudioManager.Sound.BushRustle, transform.position);

        // Replace each obstacle with a random bushPrefab in the same position
        foreach (var obstacle in obstacles)
        {
            var obstaclePos = obstacle.transform.position;
            var obstacleRot = obstacle.transform.rotation;
            var obstacleScale = obstacle.transform.localScale;
            var obstacleParent = obstacle.transform.parent;
            var obstacleLayer = obstacle.layer;
            Destroy(obstacle);

            var bushPrefabs = RoomController.Instance.bushPrefabs;

            var randomBush = Instantiate(bushPrefabs[UnityEngine.Random.Range(0, bushPrefabs.Count)], obstaclePos,
                obstacleRot, obstacleParent);
            randomBush.transform.localScale = obstacleScale;
            randomBush.layer = obstacleLayer;
        }

        // Add fireflies
        roomSpawner.AddFireflyParticles();
    }
}