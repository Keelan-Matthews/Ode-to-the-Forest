using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public Room room;

    [System.Serializable]
    public struct Grid
    {
        public int columns, rows;
        public float verticalOffset, horizontalOffset;
    }
    
    public Grid grid;
    public GameObject gridTile;
    public List<Vector2> gridPositions = new ();
    private int gridOffset = 1;
    private bool initialized = false;
    
    private void Awake()
    {
        // Get the room that this grid is attached to
        room = GetComponentInParent<Room>();
        // Set the grid size to the room size
        grid.columns = room.width - gridOffset;
        grid.rows = room.height - gridOffset;

        GenerateGrid();
    }

    private void Update()
    {
        // If the grid is not active, return
        if (!gameObject.activeSelf) return;
        
        // If the player has just entered the room, initialize the object spawning
        // if (room.activeRoom && !initialized)
        // {
        //     initialized = true;
        //
        //     // Spawn the objects in the room
        //     GetComponentInParent<ObjectRoomSpawner>().InitializeObjectSpawning();
        // }
    }

    private void GenerateGrid()
    {
        // Set the grid position to the bottom left of the room
        grid.verticalOffset += room.transform.localPosition.y;
        grid.horizontalOffset += room.transform.localPosition.x;
        
        // Loop through the grid and create a grid tile at each position
        for (var y = 0; y < grid.rows; y++)
        {
            for (var x = 0; x < grid.columns; x++)
            {
                var tile = Instantiate(gridTile, transform);
                tile.transform.position = new Vector2(x - (grid.columns - grid.horizontalOffset), y - (grid.rows - grid.verticalOffset));
                tile.name = "X: " + x + " Y: " + y;
                gridPositions.Add(tile.transform.position);
                // Hide or Show the grid here
                tile.SetActive(false);
            }
        }
    }
}
