using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour, IDataPersistence
{
    // UI Variables
    private static TextMeshProUGUI essenceText;
    public static GameManager Instance { get; private set; }

    // Make a list of prefabs for the room types
    public List<GameObject> roomPrefabs = new ();
    public MinimapRoom minimapRoomPrefab;
    public string currentWorldName = "Forest";
    public Room activeRoom;
    private const float EssenceForceDelay = 0.3f;
    [SerializeField] private GameObject permaSeedPrefab;
    public bool canDropEssence = true;
    public bool activeDialogue;
    public bool isTutorial;

    public static event Action<Room> OnStartWave;
    public static event Action OnContinue;
    
    [Header("Data Persistence")]
    [SerializeField] private bool firstLoad;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate GameManager instances
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scene changes
        
        // Subscribe to the OnRoomChange event
        RoomController.OnRoomChange += RoomController_OnRoomChange;
        // Subscribe to the OnRoomCleared event
        RoomController.OnRoomCleared += RoomController_OnRoomCleared;
    }
    
    // Unsubscribe on destroy
    // private void OnDestroy()
    // {
    //     RoomController.OnRoomChange -= RoomController_OnRoomChange;
    //     RoomController.OnRoomCleared -= RoomController_OnRoomCleared;
    //     Health.OnPlayerDeath -= Health_OnPlayerDeath;
    // }
    
    public GameObject GetRoomPrefab(string roomType)
    {
        // Find the room prefab with the same name as the room type
        var roomPrefab = roomPrefabs.Find(prefab => prefab.name == currentWorldName + roomType);
        return roomPrefab;
    }
    
    public GameObject GetMinimapRoomPrefab(string roomType)
    {
        // Return the minimap room prefab with its specific icon
        minimapRoomPrefab.SetRoomIcon(roomType);

        return minimapRoomPrefab.gameObject;
    }
    
    public void SetActiveDialogue(bool active)
    {
        activeDialogue = active;
    }
    
    private void RoomController_OnRoomChange(Room room)
    {
        // Set the active room to the new room
        activeRoom = room;

        // If the room has a tag of EnemyRoom and has not been cleared, lock the doors
        // and start the wave
        if (!room.CompareTag("EnemyRoom") || room.IsCleared()) return;
        
        room.LockRoom();
        
        // If the room has dialogue, return
        if (isTutorial && room.hasDialogue) return;
        
        if (!room.hasWave) return;
        // Wait before starting the wave
        StartCoroutine(StartWave(room));
    }

    private static IEnumerator StartWave(Room room)
    {
        yield return new WaitForSeconds(1f);
        // Get the TextMeshProUGUI component from the canvas
        essenceText = RoomController.Instance.essenceText;
        // Spawn enemies in the current room
        OnStartWave?.Invoke(room);
    }
    
    private static void RoomController_OnRoomCleared(Room room)
    {
        if (!room.CompareTag("EnemyRoom") || room.name == "WaveRoom") return;

        room.UnlockRoom();
    }

    public static void OnGameContinue()
    {
        // Invoke on continue event
        OnContinue?.Invoke();
        // Take the player back to the Home scene
        ScenesManager.LoadScene("Home");
        
        // Remove all the perma seed buffs
        PermaSeedManager.Instance.RemoveActiveSeeds();
    }

    #region Essence & Perma Seeds

    public void UpdateEssenceUI(int amount)
    {
        if (essenceText == null) return;
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
    
    public void DropPermaSeed(Vector3 position)
    {
        if (isTutorial) return;
        if (Instance.activeRoom.spawnedPermaSeed) return;
        
        // If the player already has a seed, return
        if (PermaSeedManager.Instance.HasSeed()) return;
        
        if (Random.Range(0, 100) < 95) return;

        // Instantiate a perma seed prefab at the given position
        var permaSeed = Instantiate(permaSeedPrefab, position, Quaternion.identity);
        // Set the parent 
        permaSeed.transform.SetParent(Instance.activeRoom.transform);
        
        Instance.activeRoom.spawnedPermaSeed = true;
        
        var seedRb = permaSeed.GetComponent<Rigidbody2D>();
        
        // Apply a force to the essence to make it scatter slightly
        seedRb.AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * 5f, ForceMode2D.Impulse);
        
        // Reset the velocity of the essence after a delay
        StartCoroutine(ResetVelocity(seedRb));
    }
    
    private IEnumerator ResetVelocity(Rigidbody2D essence)
    {
        yield return new WaitForSeconds(EssenceForceDelay);
        if (essence == null) yield break;
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
        if (essence == null) yield break;
        if (!essence.activeSelf) yield break;
        
        // Make the essence flash by toggling its sprite renderer
        var essenceSprite = essence.GetComponent<SpriteRenderer>();
        for (var i = 0; i < 10; i++)
        {
            if (essence == null) break;
            if (!essence.activeSelf) break;
            essenceSprite.enabled = !essenceSprite.enabled;
            yield return new WaitForSeconds(0.1f);
        }
        
        if (essence.activeSelf)
            essence.SetActive(false);

    }


    #endregion

    public void LoadData(GameData data)
    {
        isTutorial = data.IsTutorial;
    }

    public void SaveData(GameData data)
    {
        data.IsTutorial = isTutorial;
    }

    public bool FirstLoad()
    {
        return firstLoad;
    }
}
