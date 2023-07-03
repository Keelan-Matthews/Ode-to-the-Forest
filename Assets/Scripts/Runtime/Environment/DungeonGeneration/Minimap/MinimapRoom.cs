using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapRoom : MonoBehaviour
{
    private int _width = 4;
    private int _height = 2;
    public int x;
    public int y;
    
    public bool visited;
    
    // This array holds the visited and unvisited room sprites
    public Sprite[] backgrounds;
    // This holds the icons for each room along with the room type
    public Sprite[] icons;
    
    public List<MinimapRoom> connectedRooms = new ();
    public SpriteRenderer spriteRenderer;

    // getter and setter for the room width
    public int Width
    {
        get => _width;
        set => _width = value;
    }
    
    // getter and setter for the room height
    public int Height
    {
        get => _height;
        set => _height = value;
    }

    private void Start()
    {
        if (MinimapManager.Instance == null)
        {
            throw new Exception("Scene not found");
        }
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        MinimapManager.Instance.RegisterMinimapRoom(this);
    }
    
    // this function assigns the room an icon based on the room type, and centers it
    public void SetRoomIcon(string type)
    {
        var icon = Array.Find(icons, i => i.name == type);
        if (icon == null) return;
        
        // Create an icon as a child of this room
        var roomIcon = new GameObject("RoomIcon", typeof(SpriteRenderer));
        roomIcon.transform.SetParent(transform);
        roomIcon.GetComponent<SpriteRenderer>().sprite = icon;
        roomIcon.transform.localPosition = Vector3.zero;
    }
    
    public void SetVisited()
    {
        visited = true;
        spriteRenderer.sprite = backgrounds[1];
    }

    public void DetermineDoors()
    {
        if (MinimapManager.Instance.DoesRoomExist(x, y + 1))
        {
            connectedRooms.Add(MinimapManager.Instance.FindRoom(x, y + 1));
        }
        
        if (MinimapManager.Instance.DoesRoomExist(x, y - 1))
        {
            connectedRooms.Add(MinimapManager.Instance.FindRoom(x, y - 1));
        }
        
        if (MinimapManager.Instance.DoesRoomExist(x + 1, y))
        {
            connectedRooms.Add(MinimapManager.Instance.FindRoom(x + 1, y));
        }
        
        if (MinimapManager.Instance.DoesRoomExist(x - 1, y))
        {
            connectedRooms.Add(MinimapManager.Instance.FindRoom(x - 1, y));
        }
    }
    
    // This function returns the centre of the room
    public Vector3 GetRoomCentre()
    {
        return new Vector3(x * _width, y * _height, 0);
    }
    
    public void DisableIfNotVisited()
    {
        if (!visited)
        {
            spriteRenderer.enabled = false;
        }
    }
}
