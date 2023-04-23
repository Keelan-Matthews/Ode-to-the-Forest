using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI essenceText;

    public static GameManager Instance { get; private set; }
    public Room activeRoom;
    // Make a list of prefabs for the room types
    public List<GameObject> roomPrefabs = new ();
    public string currentWorldName = "Forest";

    public static event Action<Room> OnStartWave;

    private float _essenceForceDelay = 0.3f;

    private void Awake()
    {
        Instance = this;
        
        // Subscribe to the OnRoomChange event
        RoomController.OnRoomChange += RoomController_OnRoomChange;
        // Subscribe to the OnRoomCleared event
        RoomController.OnRoomCleared += RoomController_OnRoomCleared;
    }
    
    public GameObject GetRoomPrefab(string roomType)
    {
        // Find the room prefab with the same name as the room type
        var roomPrefab = roomPrefabs.Find(prefab => prefab.name == currentWorldName + roomType);
        return roomPrefab;
    }
    
    private void RoomController_OnRoomChange(Room room)
    {
        // Set the active room to the new room
        activeRoom = room;
        
        // If the room has a tag of EnemyRoom and has not been cleared, lock the doors
        // and start the wave
        if (!room.CompareTag("EnemyRoom") || room.IsCleared()) return;
        foreach (var door in room.doors)
        {
            door.LockDoor();    
        }
        
        // Wait before starting the wave
        StartCoroutine(StartWave(room));
    }
    
    private static IEnumerator StartWave(Room room)
    {
        yield return new WaitForSeconds(2f);
        // Spawn enemies in the current room
        OnStartWave?.Invoke(room);
    }
    
    private void RoomController_OnRoomCleared(Room room)
    {
        // If the room has a tag of EnemyRoom, unlock the doors
        if (!room.CompareTag("EnemyRoom")) return;
        foreach (var door in room.doors)
        {
            door.UnlockDoor();    
        }
    }

    public void UpdateEssenceUI(int amount)
    {
        essenceText.text = "Amount: " + amount;
    }
    
    public void DropEssence(int amount, Vector2 position)
    {
        for (var i = 0; i < amount; i++)
        {
            // Deposit essence
            var essence = EssencePooler.Instance.GetPooledObject();
            
            if (essence != null)
            {
                essence.transform.position = position;
                essence.SetActive(true);
                
                var essenceRb = essence.GetComponent<Rigidbody2D>();
                
                // Apply a force to the essence to make it scatter slightly
                essenceRb.AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * 5f, ForceMode2D.Impulse);
                
                // Reset the velocity of the essence after a delay
                StartCoroutine(ResetVelocity(essenceRb));
            }
        }
    }
    
    private IEnumerator ResetVelocity(Rigidbody2D essence)
    {
        yield return new WaitForSeconds(_essenceForceDelay);
        essence.velocity = Vector2.zero;
        
        // Start a coroutine that destroys the essence after a delay
        StartCoroutine(DestroyEssence(essence.gameObject));
    }
    
    private IEnumerator DestroyEssence(GameObject essence)
    {
        // Wait between 3 and 5 seconds before making the essence flash for a further 3 seconds,
        // then destroy it
        yield return new WaitForSeconds(Random.Range(3f, 6f));
        
        // If essence is still active, make it flash and then destroy it
        if (!essence.activeSelf) yield break;
        
        // Make the essence flash by toggling its sprite renderer
        var essenceSprite = essence.GetComponent<SpriteRenderer>();
        for (var i = 0; i < 10; i++)
        {
            if (!essence.activeSelf) break;
            essenceSprite.enabled = !essenceSprite.enabled;
            yield return new WaitForSeconds(0.1f);
        }
        
        if (essence.activeSelf)
            essence.SetActive(false);

    }
}
