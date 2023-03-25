using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Room : MonoBehaviour
{
    [FormerlySerializedAs("Width")] public int width;
    [FormerlySerializedAs("Height")] public int height;
    [FormerlySerializedAs("X")] public int x;
    [FormerlySerializedAs("Y")] public int y;
    private bool _updatedDoors = false;

    //Door variables
    public Door leftDoor;
    public Door rightDoor;
    public Door topDoor;
    public Door bottomDoor;
    
    public List<Door> doors = new ();

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
            switch (door.doorType)
            {
                case Door.DoorType.Left:
                    leftDoor = door;
                    break;
                case Door.DoorType.Right:
                    rightDoor = door;
                    break;
                case Door.DoorType.Top:
                    topDoor = door;
                    break;
                case Door.DoorType.Bottom:
                    bottomDoor = door;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        RoomController.Instance.RegisterRoom(this);
    }
    
    private void Update()
    {
        // If we haven't updated the doors yet and the room is the end room, update the doors
        if (!name.Contains("End") || _updatedDoors) return;
        RemoveUnconnectedDoors();
        _updatedDoors = true;
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
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(width, height));
    }

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
}
