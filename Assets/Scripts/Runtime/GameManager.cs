using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Room activeRoom;
    // Make a list of prefabs for the room types
    public List<GameObject> roomPrefabs = new ();
    public string currentWorldName = "Forest";

    private void Awake()
    {
        Instance = this;
    }
    
    public GameObject GetRoomPrefab(string roomType)
    {
        // Find the room prefab with the same name as the room type
        var roomPrefab = roomPrefabs.Find(prefab => prefab.name == currentWorldName + roomType);
        return roomPrefab;
    }
}
