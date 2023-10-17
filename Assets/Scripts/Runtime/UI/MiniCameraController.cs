using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniCameraController : MonoBehaviour
{
    public static MiniCameraController Instance;
    public Room currentRoom;
    public float moveSpeed = 10f;
    public bool isMoving = true;

    private void Awake()
    {
        Instance = this;
        
        // Subscribe to the OnRoomChange event
        // RoomController.OnRoomChange += RoomController_OnRoomChange;
    }

    // Update is called once per frame
    private void Update()
    {
        if (isMoving)
        {
            UpdatePosition();
        }
        else if (!PlayerController.Instance.IsDead())
        {
            var coords = RoomController.Instance.CalculateAverageCoordinateBetweenFurthestRooms();
            var targetPosition = new Vector3(coords.x, coords.y, transform.position.z);
            transform.position = targetPosition;
        }
        else
        {
            var coords = RoomController.Instance.CalculateAverageDistanceBetweenActiveAndBossRoom();
            var targetPosition = new Vector3(coords.x, coords.y, transform.position.z);
            transform.position = targetPosition;
        }
    }
    
    private void UpdatePosition()
    {
        if (currentRoom == null) return;
        
        var targetPosition = GetCameraTargetPosition();
        // debug log the name of the camera
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }
    
    private Vector3 GetCameraTargetPosition()
    {
        if (currentRoom == null) return Vector3.zero;
        
        var minimapRoom = MinimapManager.Instance.FindRoom(currentRoom.x, currentRoom.y);
        
        var targetPosition = minimapRoom.GetRoomCentre();
        targetPosition.z = transform.position.z;
        return targetPosition;
    }

    // private void RoomController_OnRoomChange(Room room)
    // {
    //     // Set the current room to the new room
    //     currentRoom = room;
    //     // Update the camera position
    //     UpdatePosition();
    // }
}
