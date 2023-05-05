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
        RoomController.OnLoad += UpdateIcon;
        // get the room component
        _room = GetComponentInParent<Room>();
    }

    private void UpdateIcon(Room room)
    {
        // Check if the room is the current room
        if (room != _room) return;

        // Enable the sprite renderers of the adjacent rooms
        var adjacentRooms = _room.connectedRooms;
        
        foreach (var adjacentRoom in adjacentRooms)
        {
            // Get the minimap icon controller of the adjacent room
            var minimapIconController = adjacentRoom.GetComponentInChildren<MinimapIconController>();
            // Enable the sprite renderer
            minimapIconController._spriteRenderer.enabled = true;
            // Set the color of the adjacent room to white
            minimapIconController._spriteRenderer.color = new Color(0.75f, 0.75f, 0.75f, 0.6f);
        }
        
        // Enable the sprite renderer
        _spriteRenderer.enabled = true;
            
        // Set the color of the active room to white
        _spriteRenderer.color = Color.white;
    }
    
    public void DisableIfUnvisited()
    {
        // Check if the room has been visited
        if (_room.cleared)
        {
            _spriteRenderer.color = new Color(0.75f, 0.75f, 0.75f, 0.6f);
            return;
        }
        
        // Disable the sprite renderer
        _spriteRenderer.enabled = false;
    }
}
