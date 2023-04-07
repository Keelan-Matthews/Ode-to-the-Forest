using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This script is attached to the grid game object in the room prefab.
 * It is responsible for generating the grid for the room and setting the grid positions.
 * It also gets the object room spawner and initializes the object spawning such as
 * obstacles and chests.
 */
public class GridController : MonoBehaviour
{
    public Room room;

    [Serializable]
    public struct Grid
    {
        public int columns, rows;
        public float verticalOffset, horizontalOffset;
    }
    
    public Grid grid;
    public GameObject gridTile;
    public List<Vector2> gridPositions = new ();
    private int gridOffset = 8;
    private ObjectRoomSpawner _ors;
    
    private void Awake()
    {
        // Get the room that this grid is attached to
        room = GetComponentInParent<Room>();
        // Get the object room spawner of the room
        _ors = GetComponentInParent<ObjectRoomSpawner>();
        
        // Set the grid size to the room size
        grid.columns = room.width - gridOffset;
        grid.rows = room.height - gridOffset;

        GenerateGrid();
    }

    private void GenerateGrid()
    {
        // Set the grid position to the bottom left of the room
        var localPos = room.transform.localPosition;
        grid.verticalOffset += localPos.y;
        grid.horizontalOffset += localPos.x;
        
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

        _ors.InitializeObjectSpawning();
    }
}
