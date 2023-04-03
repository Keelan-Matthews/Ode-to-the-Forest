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
    private float widthOffset = 7.5f; // Change based on width of player
    private bool _locked = false;
    private SpriteRenderer _renderer;
    
    [SerializeField] private Sprite _lockedSprite;
    [SerializeField] private Sprite _unlockedSprite;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        
        // Get the Sprite child and update the sprite to locked door
        _renderer = GetComponentInChildren<SpriteRenderer>();
    }
    
    // Teleport the player to the next room to overcome the door being blocked
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player") || _locked) return;
        player.transform.position = doorType switch
        {
            DoorType.Bottom => new Vector2(player.transform.position.x, player.transform.position.y - widthOffset),
            DoorType.Left => new Vector2(player.transform.position.x - widthOffset, player.transform.position.y),
            DoorType.Right => new Vector2(player.transform.position.x + widthOffset, player.transform.position.y),
            DoorType.Top => new Vector2(player.transform.position.x, player.transform.position.y + widthOffset),
            _ => player.transform.position
        };
    }


    public void LockDoor()
    {
        _locked = true;
        
        _renderer.sprite = _lockedSprite;
    }
    
    public void UnlockDoor()
    {
        _locked = false;
        
        // Get the Sprite child and update the sprite to unlocked door
        _renderer.sprite = _unlockedSprite;
    }
}
