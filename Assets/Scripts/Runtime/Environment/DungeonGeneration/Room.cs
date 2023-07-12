using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public List<Door> doors = new ();
    public List<Room> connectedRooms = new ();
    public GameObject roomBackground;
    private SunlightController _sunlightController;

    [Serializable]
    public struct EnemySpawnerData
    {
        public string name;
        public SpawnerData spawnerData;
    }
    
    public List<EnemySpawnerData> enemySpawners = new ();
    private static readonly int IsPurified = Animator.StringToHash("IsPurified");

    #endregion
    #region Room Variables  

    private bool _updatedDoors;
    public bool hasWave = true;
    public bool cleared;
    public bool spawnedPermaSeed;
    public bool hasDialogue;
    private bool _isPurifying;

    #endregion
    #region Room Difficulty

    public int difficulty;
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
        // SetDifficulty();
    }
    
    private void Update()
    {
        // If we haven't updated the doors yet and the room is the end room, update the doors
        if (name.Contains("End") && !_updatedDoors)
        {
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
    }

    // Set the difficulty of the room
    private void SetDifficulty()
    {
        // Get the difficulty from the room name (ForestEasy = Easy)
        // Split the room name into "Forest" and whatever is after it
        var splitName = name.Split(new[] { "Forest-" }, StringSplitOptions.None);
        var d = splitName[1].Split(' ')[0];
        switch (d)
        {
            case "Easy":
                difficulty = 0;
                waveDuration = 10f;
                break;
            case "Medium":
                difficulty = 1;
                waveDuration = 20f;
                break;
            case "Hard":
                difficulty = 2;
                waveDuration = 30f;
                break;
            default:
                break;
        }
    }

    #region Room Initialisation & Getters

     public void RemoveUnconnectedDoors()
    {
        foreach (var door in doors)
        {
            switch (door.doorType)
            {
                case Door.DoorType.Right:
                    if (GetRight() == null)
                        door.gameObject.SetActive(false);
                    else
                    {
                        connectedRooms.Add(GetRight());
                        SetDoorType(GetRight().name, Door.DoorType.Right);
                    }

                    break;
                case Door.DoorType.Bottom:
                    if (GetBottom() == null)
                        door.gameObject.SetActive(false);
                    else
                    {
                        connectedRooms.Add(GetBottom());
                        SetDoorType(GetBottom().name, Door.DoorType.Bottom);
                    }
                    break;
                case Door.DoorType.Left:
                    if (GetLeft() == null)
                        door.gameObject.SetActive(false);
                    else
                    {
                        connectedRooms.Add(GetLeft());
                        SetDoorType(GetLeft().name, Door.DoorType.Left);
                    }
                    break;
                case Door.DoorType.Top:
                    if (GetTop() == null)
                        door.gameObject.SetActive(false);
                    else
                    {
                        connectedRooms.Add(GetTop());
                        SetDoorType(GetTop().name, Door.DoorType.Top);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    // Get the room to the right of the current room
    private Room GetRight()
    {
        return !RoomController.Instance.DoesRoomExist(x + 1, y) ? null : RoomController.Instance.FindRoom(x + 1, y);
    }
    
    // Get the room to the left of the current room
    private Room GetLeft()
    {
        return !RoomController.Instance.DoesRoomExist(x - 1, y) ? null : RoomController.Instance.FindRoom(x - 1, y);
    }
    
    // Get the room above the current room
    private Room GetTop()
    {
        return !RoomController.Instance.DoesRoomExist(x, y + 1) ? null : RoomController.Instance.FindRoom(x, y + 1);
    }
    
    // Get the room below the current room
    private Room GetBottom()
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
            RoomController.Instance.OnPlayerEnterRoom(this);
        }
    }

    #region Room Spawning

    public Vector2 GetRandomPositionInRoom()
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
                randomPosition.x = randomX == 0 ? -width / 2 + WallOffset : width / 2 - WallOffset;
            
                // Randomly pick a y position within the height of the room and add or subtract the wall offset
                randomPosition.y = UnityEngine.Random.Range(-height / 2 + WallOffset, height / 2 - WallOffset);
                break;
            }
            // If the random axis is 1, constrain the y axis to a side - either top or bottom
            case 1:
            {
                var randomY = UnityEngine.Random.Range(0, 2);
                randomPosition.y = randomY == 0 ? -height / 2 + WallOffset : height / 2 - WallOffset;
            
                // Randomly pick a x position within the width of the room
                randomPosition.x = UnityEngine.Random.Range(-width / 2 + WallOffset, width / 2 - WallOffset);
                break;
            }
        }
        
        // Return the random position and offset it by the room's position
        return randomPosition + (Vector2)transform.position;
    }

    public static bool ReviewRoomPosition(Vector2 randomPos, Collider2D enemyCollider)
    {
        // Determine if the position in the room has enough space to spawn the enemy,
        // but only checking if it collides with Obstacle,Player and Wall tag and not the room collider
        
        // Calculate the expanded hit radius by adding the leeway to the enemy's bounds
        const float leeway = 0.2f;
        var expandedHitRadius = enemyCollider.bounds.extents.x + leeway;
        
        var hit = Physics2D.OverlapCircle(randomPos, expandedHitRadius, LayerMask.GetMask("Obstacle", "Player", "Wall"));
        return hit == null;
    }
    

    #endregion
    
    public void OnWaveEnd()
    {
        cleared = true;
        RoomController.Instance.OnPlayerClearRoom(this);

        if (hasWave)
        {
            _sunlightController.Expand();
        }

        GrowBackground();
        PurifyObstaclesInRoom();
        
        // Set inSunlight to true in the PlayerController script
        PlayerController.Instance.inSunlight = true;
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
        // Get all the obstacles in the room
        var obstacles = roomSpawner.GetObstacles();
        
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
            
            var randomBush = Instantiate(bushPrefabs[UnityEngine.Random.Range(0, bushPrefabs.Count)], obstaclePos, obstacleRot, obstacleParent);
            randomBush.transform.localScale = obstacleScale;
            randomBush.layer = obstacleLayer;
        }
    }
}
