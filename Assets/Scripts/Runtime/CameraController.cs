using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    public Room currentRoom;
    private float moveSpeed = 100f;
    public bool followPlayer;
    public bool panToMother;
    public BoxCollider2D boundBox;
    private Vector3 _minBounds;
    private Vector3 _maxBounds;
    
    private float _halfHeight;
    private float _halfWidth;
    private bool _hasBounds;

    private void Awake()
    {
        Instance = this;
        
        if (boundBox != null)
        {
            _hasBounds = true;
            // There is a bound box, so set the min and max bounds
            var bounds = boundBox.bounds;
            _minBounds = bounds.min;
            _maxBounds = bounds.max;
            
            // Get the camera which is a sibling of this object
            _halfHeight = GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize;
            _halfWidth = _halfHeight * Screen.width / Screen.height;
        }
    }

    // private void Start()
    // {
    //     // DontDestroyOnLoad(transform.gameObject);
    // }

    // Update is called once per frame
    private void Update()
    {
        UpdatePosition();

        if (!_hasBounds) return;
        var position = transform.position;
        var clampedX = Mathf.Clamp(position.x, _minBounds.x + _halfWidth, _maxBounds.x - _halfWidth);
        var clampedY = Mathf.Clamp(position.y, _minBounds.y + _halfHeight, _maxBounds.y - _halfHeight);
        transform.position = new Vector3(clampedX, clampedY, position.z);
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
