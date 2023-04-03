using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

public class Room : MonoBehaviour
{
    [FormerlySerializedAs("Width")] public int width;
    [FormerlySerializedAs("Height")] public int height;
    [FormerlySerializedAs("X")] public int x;
    [FormerlySerializedAs("Y")] public int y;
    private bool _updatedDoors = false;
    private int _difficulty;
    public bool cleared = false;
    public float waveStartTime;
    private float wallOffset = 2f;

    public List<Door> doors = new ();
    
    public float waveDuration = 10f;
    
    [Serializable]
    public struct EnemySpawnerData
    {
        public string name;
        public SpawnerData spawnerData;
    }
    
    public List<EnemySpawnerData> enemySpawners = new ();

    // Start is called before the first frame update
    private void Start()
    {
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
    
    private void Update()
    {
        // If we haven't updated the doors yet and the room is the end room, update the doors
        if (!name.Contains("End") || _updatedDoors) return;
        RemoveUnconnectedDoors();
        _updatedDoors = true;
    }
    
    // Set the difficulty of the room
    private void SetDifficulty()
    {
        // Randomly choose a difficulty
        var difficulty = UnityEngine.Random.Range(1, 4);
        _difficulty = difficulty;
    }
    
    public void RemoveUnconnectedDoors()
    {
        foreach (var door in doors)
        {
            switch (door.doorType)
            {
                case Door.DoorType.Right:
                    if (GetRight() == null)
                        door.gameObject.SetActive(false);
                    break;
                case Door.DoorType.Bottom:
                    if (GetBottom() == null)
                        door.gameObject.SetActive(false);
                    break;
                case Door.DoorType.Left:
                    if (GetLeft() == null)
                        door.gameObject.SetActive(false);
                    break;
                case Door.DoorType.Top:
                    if (GetTop() == null)
                        door.gameObject.SetActive(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
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

    // Draw Gizmos in the scene view for testing purposes
    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireCube(transform.position, new Vector3(width, height));
    // }

    public Vector3 GetRoomCentre()
    {
        return new Vector3(x * width, y * height);
    }

    // Set the camera to the current room when the player enters the room
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            RoomController.Instance.OnPlayerEnterRoom(this);
        }
    }
    
    public int GetDifficulty()
    {
        return _difficulty;
    }
    
    public Vector2 GetRandomPositionInRoom()
    {
        // Randomly pick which axis will be constrained to a side
        var randomAxis = UnityEngine.Random.Range(0, 2);
        var randomPosition = new Vector2();
        
        // If the random axis is 0, constrain the x axis to a side - either left or right
        if (randomAxis == 0)
        {
            var randomX = UnityEngine.Random.Range(0, 2);
            // Constrain the axis to a side and either add or subtract the wall offset
            randomPosition.x = randomX == 0 ? -width / 2 + wallOffset : width / 2 - wallOffset;
            
            // Randomly pick a y position within the height of the room and add or subtract the wall offset
            randomPosition.y = UnityEngine.Random.Range(-height / 2 + wallOffset, height / 2 - wallOffset);
        }
        // If the random axis is 1, constrain the y axis to a side - either top or bottom
        else if (randomAxis == 1)
        {
            var randomY = UnityEngine.Random.Range(0, 2);
            randomPosition.y = randomY == 0 ? -height / 2 + wallOffset : height / 2 - wallOffset;
            
            // Randomly pick a x position within the width of the room
            randomPosition.x = UnityEngine.Random.Range(-width / 2 + wallOffset, width / 2 - wallOffset);
        }
        
        // Return the random position and offset it by the room's position
        return randomPosition + (Vector2)transform.position;
    }
    
    public void OnWaveEnd()
    {
        cleared = true;
        RoomController.Instance.OnPlayerClearRoom(this);
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
}
