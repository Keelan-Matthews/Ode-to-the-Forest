using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapIconController : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Room _room;
    // Start is called before the first frame update
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        // Subscribe to OnRoomChange event
        RoomController.OnRoomChange += UpdateIcon;
        // get the room component
        _room = GetComponentInParent<Room>();
    }

    private void UpdateIcon(Room room)
    {
        // Check if the room is the current room
        if (room == _room)
        {
            // Disable the sprite renderer
            _spriteRenderer.enabled = true;
        }
    }
}
