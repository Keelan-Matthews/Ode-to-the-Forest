using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    public Room currentRoom;
    public float moveSpeed = 100f;

    private void Awake()
    {
        Instance = this;
        
        // Subscribe to the OnRoomChange event
        // RoomController.OnRoomChange += RoomController_OnRoomChange;
    }

    // Update is called once per frame
    private void Update()
    {
        UpdatePosition();
    }
    
    private void UpdatePosition()
    {
        if (currentRoom == null) return;
        
        var targetPosition = GetCameraTargetPosition();
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }
    
    private Vector3 GetCameraTargetPosition()
    {
        if (currentRoom == null) return Vector3.zero;
        
        var targetPosition = currentRoom.GetRoomCentre();
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
