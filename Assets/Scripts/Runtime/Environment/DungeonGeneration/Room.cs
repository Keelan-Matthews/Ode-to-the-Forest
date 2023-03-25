using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int Width;
    public int Height;
    public int X;
    public int Y;
    
    // Start is called before the first frame update
    void Start()
    {
        if (RoomController.Instance == null)
        {
            throw new Exception("Scene not found");
        }
        
        RoomController.Instance.RegisterRoom(this);
    }

    // Draw Gizmos in the scene view for testing purposes
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(Width, Height));
    }

    public Vector3 GetRoomCentre()
    {
        return new Vector3(X * Width, Y * Height);
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
