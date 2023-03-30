using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public enum DoorType
    {
        Left,
        Right,
        Top,
        Bottom
    }
    
    public DoorType doorType;
    private GameObject player;
    private float widthOffset = 7f; // Change based on width of player
    public GameObject doorCollider;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    
    // Teleport the player to the next room to overcome the door being blocked
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        player.transform.position = doorType switch
        {
            DoorType.Bottom => new Vector2(transform.position.x, transform.position.y - widthOffset),
            DoorType.Left => new Vector2(transform.position.x - widthOffset, transform.position.y),
            DoorType.Right => new Vector2(transform.position.x + widthOffset, transform.position.y),
            DoorType.Top => new Vector2(transform.position.x, transform.position.y + widthOffset),
            _ => player.transform.position
        };
    }
}
