using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    // UI Variables
    [SerializeField] private TextMeshProUGUI essenceText;

    public static GameManager Instance { get; private set; }

    // Make a list of prefabs for the room types
    public List<GameObject> roomPrefabs = new ();
    public string currentWorldName = "Forest";
    public Room activeRoom;
    private const float EssenceForceDelay = 0.3f;

    public static event Action<Room> OnStartWave;

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
        yield return new WaitForSeconds(1f);
        // Spawn enemies in the current room
        OnStartWave?.Invoke(room);
    }
    
    private static void RoomController_OnRoomCleared(Room room)
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
        essenceText.text = amount.ToString();
    }
    
    public void DropEssence(int amount, Vector2 position)
    {
        // Generate a random number between 0 and amount
        var randomAmount = Random.Range(1, amount + 1);
        for (var i = 0; i < randomAmount; i++)
        {
            // Deposit essence
            var essence = EssencePooler.Instance.GetPooledObject();

            if (essence == null) continue;
            essence.transform.position = position;
            essence.SetActive(true);
                
            var essenceRb = essence.GetComponent<Rigidbody2D>();
                
            // Apply a force to the essence to make it scatter slightly
            essenceRb.AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * 5f, ForceMode2D.Impulse);
                
            // Reset the velocity of the essence after a delay
            StartCoroutine(ResetVelocity(essenceRb));
        }
    }
    
    private IEnumerator ResetVelocity(Rigidbody2D essence)
    {
        yield return new WaitForSeconds(EssenceForceDelay);
        essence.velocity = Vector2.zero;
        
        // Start a coroutine that destroys the essence after a delay
        StartCoroutine(DestroyEssence(essence.gameObject));
    }
    
    private static IEnumerator DestroyEssence(GameObject essence)
    {
        // Wait between 3 and 5 seconds before making the essence flash for a further 3 seconds,
        // then destroy it
        yield return new WaitForSeconds(Random.Range(4f, 7f));
        
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
