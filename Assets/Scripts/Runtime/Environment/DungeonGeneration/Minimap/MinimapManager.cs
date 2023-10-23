using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class MinimapManager : MonoBehaviour
{
    private readonly Queue<RoomInfo> _minimapRooms = new();
    private RoomInfo _currentLoadRoomData;
    private bool _isLoadingRoom;
    private bool _updatedRooms;
    public List<MinimapRoom> loadedRooms = new();
    public bool showMinimap;
    public GameObject minimapTexture;
    public Camera minimapCamera;
    private bool _minimapExpanded;
    public GameObject minimapBackground;
    public GameObject prompt;
    public bool hasEnabledAllRooms;

    private int bossX;
    private int bossY;

    public static MinimapManager Instance;

    private void Awake()
    {
        Instance = this;

        RoomController.OnRoomChange += UpdateIcon;
        RoomController.OnLoad += UpdateIcon;
    }

    private void OnDestroy()
    {
        RoomController.OnRoomChange -= UpdateIcon;
        RoomController.OnLoad -= UpdateIcon;
    }

    private void UpdateIcon(Room obj)
    {
        // Get the corresponding minimap room
        var minimapRoom = FindRoom(obj.x, obj.y);
        // Return if the room is null
        if (minimapRoom == null) return;

        // Enable this room
        minimapRoom.spriteRenderer.enabled = true;
        minimapRoom.iconRenderer.enabled = true;
        // Set this room to visited 
        minimapRoom.SetVisited();

        // If the player is currently in the room, set the icon to "Active", else set it to,
        // the rooms original icon
        if (obj.x == GameManager.Instance.activeRoom.x && obj.y == GameManager.Instance.activeRoom.y)
        {
            minimapRoom.SetRoomIcon("Active");
        }

        // Enable the adjacent rooms
        var adjacentRooms = minimapRoom.connectedRooms;

        foreach (var adjacentRoom in adjacentRooms)
        {
            adjacentRoom.spriteRenderer.enabled = true;

            // If an adjacent room name contains "End", set the icon to the boss icon prematurely
            if (adjacentRoom.name.Contains("End"))
            {
                adjacentRoom.iconRenderer.enabled = true;
            }

            // If the adjacent room is visited, set it back to its original icon
            if (adjacentRoom.visited)
            {
                // Extract "Start" from the component name Minimap-Start 0,0
                var roomType = adjacentRoom.name.Split('-')[1];
                // remove the coordinates from the room type
                roomType = roomType.Split(' ')[0];
                adjacentRoom.SetRoomIcon(roomType);
            }
            else
            {
                if (hasEnabledAllRooms) return;
                var thisRoom = GetRoomFromMinimapRoom(minimapRoom);
                var adjRoom = GetRoomFromMinimapRoom(adjacentRoom);

                if (thisRoom.GetRight() == adjRoom)
                {
                    if (!minimapRoom.doorRight.activeSelf)
                    {
                        adjacentRoom.spriteRenderer.enabled = false;
                    }
                }
                else if (thisRoom.GetLeft() == adjRoom)
                {
                    if (!minimapRoom.doorLeft.activeSelf)
                    {
                        adjacentRoom.spriteRenderer.enabled = false;
                    }
                }
                else if (thisRoom.GetTop() == adjRoom)
                {
                    if (!minimapRoom.doorTop.activeSelf)
                    {
                        adjacentRoom.spriteRenderer.enabled = false;
                    }
                }
                else if (thisRoom.GetBottom() == adjRoom)
                {
                    if (!minimapRoom.doorBottom.activeSelf)
                    {
                        adjacentRoom.spriteRenderer.enabled = false;
                    }
                }
            }
        }
    }

    private void Update()
    {
        // Listen for tab and expand or shrink the minimap
        if (Input.GetKeyDown(KeyCode.Tab) && !PlayerController.Instance.IsDead())
        {
            _minimapExpanded = !_minimapExpanded;

            if (_minimapExpanded)
            {
                ExpandMinimap();
            }
            else
            {
                ShrinkMinimap();
            }
        }

        // Show or hide the minimap based on the showMinimap bool
        minimapTexture.SetActive(showMinimap);

        // If all the rooms are loaded, determine their doors
        if (!_updatedRooms && RoomController.Instance.updatedRooms)
        {
            // For each room in the loaded rooms
            foreach (var room in loadedRooms)
            {
                room.DetermineDoors();
            }
            
            // Update the minimap
            UpdateIcon(GameManager.Instance.activeRoom);
            
            var startRoom = FindRoom(0, 0);
            startRoom.UpdateDoors();

            _updatedRooms = true;
        }

        // Return if the room is loading or the queue is empty
        if (_isLoadingRoom || _minimapRooms.Count == 0) return;

        _currentLoadRoomData = _minimapRooms.Dequeue();
        _isLoadingRoom = true;

        // Add the current room prefab to the scene at the specified position
        var roomPrefab = GameManager.Instance.GetMinimapRoomPrefab(_currentLoadRoomData.Name);
        Instantiate(roomPrefab, new Vector3(_currentLoadRoomData.X, _currentLoadRoomData.Y, 0), Quaternion.identity);
    }

    public void LoadMinimapRoom(string roomName, int x, int y)
    {
        if (DoesRoomExist(x, y)) return;

        var newRoomData = new RoomInfo
        {
            Name = roomName,
            X = x,
            Y = y
        };

        _minimapRooms.Enqueue(newRoomData);

        if (roomName.Contains("End"))
        {
            bossX = x;
            bossY = y;
        }
    }

    public void SetBossRoomVisited()
    {
        var room = FindRoom(bossX, bossY);
        if (room == null) return;

        room.SetVisited();
        // Enable the boss icon and sprite renderer
        room.iconRenderer.enabled = true;
        room.spriteRenderer.enabled = true;
    }

    public bool DoesRoomExist(int x, int y)
    {
        return loadedRooms.Find(r => r.x == x && r.y == y) != null;
    }

    public void RegisterMinimapRoom(MinimapRoom room)
    {
        // If the room already exists, destroy it
        if (DoesRoomExist(_currentLoadRoomData.X, _currentLoadRoomData.Y))
        {
            Destroy(room.gameObject);
            _isLoadingRoom = false;
            return;
        }

        // Set the room's position and name
        var transform1 = room.transform;
        transform1.position = new Vector3(
            _currentLoadRoomData.X * room.Width,
            _currentLoadRoomData.Y * room.Height,
            0
        );

        room.x = _currentLoadRoomData.X;
        room.y = _currentLoadRoomData.Y;
        room.name = "Minimap-" + _currentLoadRoomData.Name + " " + _currentLoadRoomData.X + "," +
                    _currentLoadRoomData.Y;
        transform1.parent = transform;

        _isLoadingRoom = false;

        loadedRooms.Add(room);
    }

    public MinimapRoom FindRoom(int x, int y)
    {
        return loadedRooms.Find(r => r.x == x && r.y == y);
    }

    public void UpdateAllRooms()
    {
        foreach (var obj in loadedRooms)
        {
            var minimapRoom = FindRoom(obj.x, obj.y);
            // Return if the room is null
            if (minimapRoom == null) return;
            // Enable this room
            minimapRoom.spriteRenderer.enabled = true;
            minimapRoom.iconRenderer.enabled = true;
            // Set this room to visited 
            minimapRoom.SetVisited();
        }
    }
    
    public void EnableAllRooms()
    {
        foreach (var obj in loadedRooms)
        {
            var minimapRoom = FindRoom(obj.x, obj.y);
            // Return if the room is null
            if (minimapRoom == null) return;
            // Enable this room
            minimapRoom.spriteRenderer.enabled = true;
            minimapRoom.iconRenderer.enabled = true;
        }
        
        hasEnabledAllRooms = true;
    }
    
    public void EnableSpecialRooms()
    {
        foreach (var obj in loadedRooms)
        {
            var minimapRoom = FindRoom(obj.x, obj.y);
            // Return if the room is null
            if (minimapRoom == null) return;
            // Enable this room if its name contains "VendingMachine", "Shrine", "Trader", "Collector" or "Portal"
            if (minimapRoom.name.Contains("VendingMachine") || minimapRoom.name.Contains("ShrineOfYouth") || minimapRoom.name.Contains("Trader") || minimapRoom.name.Contains("Collector") || minimapRoom.name.Contains("Portal"))
            {
                minimapRoom.spriteRenderer.enabled = true;
                minimapRoom.iconRenderer.enabled = true;
            }
        }
        
        hasEnabledAllRooms = true;
    }

    public void ExpandMinimap()
    {
        // Set the render texture to be 1000x1000 and change the ortho size of the camera to 30
        minimapTexture.GetComponent<RectTransform>().sizeDelta = new Vector2(1920, 1080);

        minimapCamera.aspect = 1720f / 880f;
        minimapCamera.orthographicSize = 8f;

        // Scale this gameobject to 0.7
        gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0f);

        // For each loaded room, lower the opacity of the sprite renderer
        foreach (var room in loadedRooms)
        {
            room.spriteRenderer.color = new Color(1f, 1f, 1f, 0.7f);
            var childSpriteRenderers = room.GetComponentsInChildren<SpriteRenderer>();
            
            foreach (var childSpriteRenderer in childSpriteRenderers)
            {
                var oldColor = childSpriteRenderer.color;
                childSpriteRenderer.color = new Color(oldColor.r, oldColor.g, oldColor.b, 0.7f);
            }
        }

        // Center the rect transform of the render texture
        minimapTexture.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

        prompt.SetActive(false);
        MiniCameraController.Instance.isMoving = false;
        AudioManager.PlaySound(AudioManager.Sound.ShowMenu, transform.position);
    }

    public void SetMinimapDeathScreen()
    {
        minimapCamera.aspect = 596f / 292f;

        var horizontalDistance = CalculateHorizontalDistanceBetweenStartAndBossRoom();
        var verticalDistance = CalculateVerticalDistanceBetweenStartAndBossRoom();

        var padding = 2f;

        // Calculate the orthographic size based on the larger of horizontal or vertical distances
        float orthoSize;

        if (horizontalDistance > verticalDistance) {
            // Horizontal distance is larger, use it to calculate ortho size
            orthoSize = 0.5f * (horizontalDistance / minimapCamera.aspect) + padding;
        } else {
            // Vertical distance is larger, use it to calculate ortho size
            orthoSize = 0.5f * verticalDistance + padding;
        }

        minimapCamera.orthographicSize = orthoSize;
        
        // Ensure the orthographic size doesn't go below a minimum value
        var minOrthoSize = 1.0f; // Set your desired minimum orthographic size
        minimapCamera.orthographicSize = Mathf.Max(minimapCamera.orthographicSize, minOrthoSize);

        // Hide the minimap
        minimapTexture.SetActive(false);

        // SetBossRoomVisited();
        EnableAllRooms();
        
        MiniCameraController.Instance.isMoving = false;
    }


    public void ShrinkMinimap()
    {
        // Set the render texture to be 500x500 and change the ortho size of the camera to 15
        minimapTexture.GetComponent<RectTransform>().sizeDelta = new Vector2(180, 180);

        // Change the width and height of the camera to 180 and 180
        minimapCamera.aspect = 1;
        minimapCamera.orthographicSize = 6f;

        // Scale this gameobject to 1
        gameObject.transform.localScale = new Vector3(1f, 1f, 0f);

        foreach (var room in loadedRooms)
        {
            room.spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            room.GetComponentInChildren<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        }

        // Move the rect transfom of the render texture pos x and pos y
        minimapTexture.GetComponent<RectTransform>().anchoredPosition = new Vector2(818f, 394.2f);

        prompt.SetActive(true);
        MiniCameraController.Instance.isMoving = true;
    }

    public Room GetRoomFromMinimapRoom(MinimapRoom room)
    {
        // Get the room from the minimap room
        var roomName = room.name.Split('-')[1];
        roomName = roomName.Split(' ')[0];
        var roomX = room.name.Split(' ')[1].Split(',')[0];
        var roomY = room.name.Split(' ')[1].Split(',')[1];

        // Get the room from the room controller
        var roomFromMinimapRoom = RoomController.Instance.FindRoom(int.Parse(roomX), int.Parse(roomY));

        return roomFromMinimapRoom;
    }

    public Vector2 CalculateAverageCoordinateBetweenFurthestRooms()
    {
        if (loadedRooms.Count < 2)
        {
            Debug.LogError("There must be at least two rooms to calculate the average coordinate.");
            return Vector2.zero;
        }

        var maxDistance = 0f;
        var averageCoordinate = Vector2.zero;

        for (var i = 0; i < loadedRooms.Count - 1; i++)
        {
            for (var j = i + 1; j < loadedRooms.Count; j++)
            {
                var distance = Vector2.Distance(loadedRooms[i].transform.position,
                    loadedRooms[j].transform.position);

                if (!(distance > maxDistance)) continue;
                maxDistance = distance;

                // Calculate the average coordinate using global positions
                var averageX = (loadedRooms[i].transform.position.x + loadedRooms[j].transform.position.x) / 2f;
                var averageY = (loadedRooms[i].transform.position.y + loadedRooms[j].transform.position.y) / 2f;
                averageCoordinate = new Vector2(averageX, averageY);
            }
        }

        return averageCoordinate;
    }

    // public float CalculateDistanceBetweenStartAndBossRoom()
    // {
    //     var startRoomTransform = FindRoom(0, 0).transform;
    //     var startRoomGlobalPosition = startRoomTransform.TransformPoint(Vector3.zero);
    //     
    //     var bossRoomTransform = FindRoom(bossX, bossY).transform;
    //     var bossRoomGlobalPosition = bossRoomTransform.TransformPoint(Vector3.zero); // Get global position
    //
    //     var distance = Vector2.Distance(startRoomGlobalPosition, bossRoomGlobalPosition);
    //
    //     return distance;
    // }
    
    public float CalculateVerticalDistanceBetweenStartAndBossRoom()
    {
        var startRoomTransform = FindRoom(0, 0).transform;
        var startRoomGlobalPosition = startRoomTransform.TransformPoint(Vector3.zero);

        var bossRoomTransform = FindRoom(bossX, bossY).transform;
        var bossRoomGlobalPosition = bossRoomTransform.TransformPoint(Vector3.zero);

        var verticalDistance = Mathf.Abs(startRoomGlobalPosition.y - bossRoomGlobalPosition.y);

        return verticalDistance;
    }

    public float CalculateHorizontalDistanceBetweenStartAndBossRoom()
    {
        var startRoomTransform = FindRoom(0, 0).transform;
        var startRoomGlobalPosition = startRoomTransform.TransformPoint(Vector3.zero);

        var bossRoomTransform = FindRoom(bossX, bossY).transform;
        var bossRoomGlobalPosition = bossRoomTransform.TransformPoint(Vector3.zero);

        var horizontalDistance = Mathf.Abs(startRoomGlobalPosition.x - bossRoomGlobalPosition.x);

        return horizontalDistance;
    }

    public Vector2 CalculateAverageDistanceBetweenStartAndBossRoom()
    {
        // Find the minimap room equivalent of the active room
        var startMinimapRoom = FindRoom(0, 0).transform;
        var bossRoomTransform = FindRoom(bossX, bossY).transform;

        var averageX = (startMinimapRoom.position.x + bossRoomTransform.position.x) / 2f;
        var averageY = (startMinimapRoom.position.y + bossRoomTransform.position.y) / 2f;

        var averageCoordinate = new Vector2(averageX, averageY);

        return averageCoordinate;
    }

    public float CalculateDistanceBetweenActiveAndBossRoom()
    {
        var activeRoomTransform = GameManager.Instance.activeRoom.transform;
        var activeRoomGlobalPosition = activeRoomTransform.TransformPoint(Vector3.zero); // Get global position

        var bossRoomTransform = FindRoom(bossX, bossY).transform;
        var bossRoomGlobalPosition = bossRoomTransform.TransformPoint(Vector3.zero); // Get global position

        var distance = Vector2.Distance(activeRoomGlobalPosition, bossRoomGlobalPosition);

        return distance;
    }
}