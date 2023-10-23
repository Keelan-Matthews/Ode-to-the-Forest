using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    
    // Doors
    public GameObject doorTop;
    public GameObject doorBottom;
    public GameObject doorLeft;
    public GameObject doorRight;
    
    public List<MinimapRoom> connectedRooms = new ();
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer iconRenderer;

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
        if (icon == null)
        {
            iconRenderer.enabled = false;
        }
        
        // set the icon sprite
        iconRenderer.sprite = icon;
    }
    
    public void SetVisited()
    {
        if (visited) return;
        visited = true;
        spriteRenderer.sprite = backgrounds[1];
        UpdateDoors();
    }

    public void UpdateDoors()
    {
        // Enable the sprite renderer of the active doors
        if (doorTop.activeSelf)
        {
            doorTop.GetComponent<SpriteRenderer>().enabled = true;
        }
        
        if (doorBottom.activeSelf)
        {
            doorBottom.GetComponent<SpriteRenderer>().enabled = true;
        }
        
        if (doorLeft.activeSelf)
        {
            doorLeft.GetComponent<SpriteRenderer>().enabled = true;
        }
        
        if (doorRight.activeSelf)
        {
            doorRight.GetComponent<SpriteRenderer>().enabled = true;
        }
    }
    
    public void SetPurified()
    {
        spriteRenderer.sprite = backgrounds[2];
        
        // Enable the sprite renderer of the active doors
        if (doorTop.activeSelf)
        {
            doorTop.GetComponent<SpriteRenderer>().color = new Color(0.5725f, 0.7294f, 0.3921f);
        }
        
        if (doorBottom.activeSelf)
        {
            doorBottom.GetComponent<SpriteRenderer>().color = new Color(0.5725f, 0.7294f, 0.3921f);
        }
        
        if (doorLeft.activeSelf)
        {
            doorLeft.GetComponent<SpriteRenderer>().color = new Color(0.5725f, 0.7294f, 0.3921f);
        }
        
        if (doorRight.activeSelf)
        {
            doorRight.GetComponent<SpriteRenderer>().color = new Color(0.5725f, 0.7294f, 0.3921f);
        }
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

        var roomDoors = MinimapManager.Instance.GetRoomFromMinimapRoom(this).doors;
        
        // Enable the doors that are active
        foreach (var door in roomDoors.Where(door => door.isActive))
        {
            switch (door.doorType)
            {
                case Door.DoorType.Top:
                    doorTop.SetActive(true);
                    break;
                case Door.DoorType.Bottom:
                    doorBottom.SetActive(true);
                    break;
                case Door.DoorType.Left:
                    doorLeft.SetActive(true);
                    break;
                case Door.DoorType.Right:
                    doorRight.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    // This function returns the centre of the room
    public Vector3 GetRoomCentre()
    {
        return new Vector3(x * _width, y * _height, 0);
    }
    
    public void DisableIfNotVisited()
    {
        if (MinimapManager.Instance.hasEnabledAllRooms) return;
        if (!visited)
        {
            spriteRenderer.enabled = false;
            iconRenderer.enabled = false;
        }
    }
}
