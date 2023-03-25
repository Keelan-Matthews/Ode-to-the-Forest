using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomInfo
{
    public string Name;
    public int X;
    public int Y;
}

public class RoomController : MonoBehaviour
{
    public static RoomController Instance;
    string currentWorldName = "Forest";
    RoomInfo currentLoadRoomData;
    public Room currRoom;
    Queue<RoomInfo> loadRoomQueue = new ();
    public List<Room> loadedRooms = new ();
    bool isLoadingRoom = false;

    void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        // Load the next room in the queue
        if (isLoadingRoom) return;
        if (loadRoomQueue.Count == 0) return;
        
        currentLoadRoomData = loadRoomQueue.Dequeue();
        isLoadingRoom = true;
        StartCoroutine(LoadRoomRoutine(currentLoadRoomData));
    }

    public void LoadRoom(string name, int x, int y)
    {
        if (DoesRoomExist(x, y)) return;
        
        var newRoomData = new RoomInfo();
        newRoomData.Name = name;
        newRoomData.X = x;
        newRoomData.Y = y;
        
        loadRoomQueue.Enqueue(newRoomData);
    }

    private IEnumerator LoadRoomRoutine(RoomInfo info)
    {
        var roomName = currentWorldName + info.Name;
        var loadRoom = SceneManager.LoadSceneAsync(roomName, LoadSceneMode.Additive);
        
        while(loadRoom.isDone == false)
        {
            yield return null;
        }
    }
    
    public void RegisterRoom(Room room)
    {
        room.transform.position = new Vector3(
            currentLoadRoomData.X * room.Width,
            currentLoadRoomData.Y * room.Height,
            0
        );
        
        room.X = currentLoadRoomData.X;
        room.Y = currentLoadRoomData.Y;
        room.name = currentWorldName + "-" + currentLoadRoomData.Name + " " + currentLoadRoomData.X + "," + currentLoadRoomData.Y;
        room.transform.parent = transform;
        
        isLoadingRoom = false;
        
        if(loadedRooms.Count == 0)
            CameraController.Instance.currentRoom = room;
        
        loadedRooms.Add(room);
    }

    public bool DoesRoomExist(int x, int y)
    {
        return loadedRooms.Find(r => r.X == x && r.Y == y) != null;
    }
    
    public void OnPlayerEnterRoom(Room room)
    {
        CameraController.Instance.currentRoom = room;
        currRoom = room;
    }
}
