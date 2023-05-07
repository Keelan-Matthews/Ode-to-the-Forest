using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    public Room currentRoom;
    public float moveSpeed;
    public bool followPlayer = false;
    public bool panToMother = false;

    private void Awake()
    {
        Instance = this;
        
        // Subscribe to the OnRoomChange event
        // RoomController.OnRoomChange += RoomController_OnRoomChange;
    }

    // private void Start()
    // {
    //     DontDestroyOnLoad(transform.gameObject);
    // }

    // Update is called once per frame
    private void Update()
    {
        UpdatePosition();
    }
    
    private void UpdatePosition()
    {
        if (currentRoom == null && !followPlayer) return;
        Vector3 targetPosition;
        if (followPlayer)
        {
            targetPosition = panToMother ? GetMotherPosition() : GetPlayerPosition();
        }
        else
        {
            targetPosition = GetCameraTargetPosition();
        }
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }
    
    public void SetFollowPlayer(bool follow)
    {
        followPlayer = follow;
    }
    
    private Vector3 GetCameraTargetPosition()
    {
        if (currentRoom == null) return Vector3.zero;

        moveSpeed = 100f;
        
        var targetPosition = currentRoom.GetRoomCentre();
        targetPosition.z = transform.position.z;
        return targetPosition;
    }
    
    private Vector3 GetPlayerPosition()
    {
        if (PlayerController.Instance == null) return Vector3.zero;
        var playerPosition = PlayerController.Instance.transform.position;
        playerPosition.z = transform.position.z;
        return playerPosition;
    }

    private Vector3 GetMotherPosition()
    {
        // Get the "Mother" GameObject
        var mother = GameObject.Find("Mother");
        
        // If the "Mother" GameObject is null, return Vector3.zero
        if (mother == null) return Vector3.zero;
        
        // Get the position of the "Mother" GameObject
        var motherPosition = mother.transform.position;
        motherPosition.z = transform.position.z;

        moveSpeed = 20f;
        
        return motherPosition;
    }

    // private void RoomController_OnRoomChange(Room room)
    // {
    //     // Set the current room to the new room
    //     currentRoom = room;
    //     // Update the camera position
    //     UpdatePosition();
    // }
}
