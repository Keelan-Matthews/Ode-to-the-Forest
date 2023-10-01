using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour, IDataPersistence
{
    // UI Variables
    private static TextMeshProUGUI essenceText;
    public static GameManager Instance { get; private set; }

    // Make a list of prefabs for the room types
    public List<GameObject> roomPrefabs = new();
    public MinimapRoom minimapRoomPrefab;
    public string currentWorldName = "Forest";
    public Room activeRoom;
    private const float EssenceForceDelay = 0.3f;
    [SerializeField] private GameObject permaSeedPrefab;
    public bool canDropEssence = true;
    public bool activeDialogue;
    public bool isTutorial;
    public bool IsClearSkies;
    public bool IsSellYourSoul;
    public bool HasSeenTrader;
    public bool HasSeenCollector;
    public bool gameFinished;
    public bool deeperPortalSpawn;
    public bool deeperPortalSpawnPrompted;
    
    // Shrine of youth
    public bool[] fountainsActivated = new bool[4];

    public static event Action<Room> OnStartWave;
    public static event Action OnContinue;
    public static event Action<int> OnDiscount;
    public static event Action OnSellYourSoul;
    public static event Action OnRemoveSellYourSoul;

    [Header("Data Persistence")] [SerializeField]
    private bool firstLoad;

    [Header("Mouse Sprites")] [SerializeField]
    private Texture2D shootTexture;

    [SerializeField] private Texture2D cantShootTexture;
    private CursorMode cursorMode = CursorMode.Auto;
    private Vector2 hotSpot = Vector2.zero;

    [Header("Seed probabilities")] [SerializeField]
    private float commonSeedProbability = 0.4f;

    [SerializeField] private float rareSeedProbability = 0.2f;
    [SerializeField] private float legendarySeedProbability = 0.1f;
    public float luckModifier = 1f;

    public bool AlmanacOpen;

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
        // RoomController.OnRoomCleared += RoomController_OnRoomCleared;
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
    
    public void TriggerDiscount(int discount)
    {
        OnDiscount?.Invoke(discount);
    }
    
    public void TriggerSellYourSoul()
    {
        OnSellYourSoul?.Invoke();
        IsSellYourSoul = true;
    }
    
    public void UntriggerSellYourSoul()
    {
        OnRemoveSellYourSoul?.Invoke();
        IsSellYourSoul = false;
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
    
    public static void SetEssenceText(TextMeshProUGUI text)
    {
        essenceText = text;
    }
    
    public void ExposeCores()
    {
        // Get bossRoom tag
        var bossRoom = GameObject.FindGameObjectWithTag("bossRoom");
        bossRoom.GetComponent<BossRoomController>().ExposeCores();
    }
    
    public void HideCores()
    {
        // Get bossRoom tag
        var bossRoom = GameObject.FindGameObjectWithTag("bossRoom");
        bossRoom.GetComponent<BossRoomController>().HideCores();
    }

    // private static void RoomController_OnRoomCleared(Room room)
    // {
    //     if (!room.CompareTag("EnemyRoom") || room.name == "WaveRoom") return;
    //
    //     room.UnlockRoom();
    // }

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
        essenceText.enabled = false;
        essenceText.text = amount.ToString();
        essenceText.enabled = true;
    }

    public void DropEssence(int amount, Vector2 position)
    {

        for (var i = 0; i < amount; i++)
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

        // If the player has all the possible seeds, return
        if (PermaSeedManager.Instance.HasAllSeeds()) return;

        // If the room is easy -> there is a chance to drop a common seed
        // If the room is medium -> there is a chance to drop a rare seed
        // If the room is hard -> there is a chance to drop an epic seed
        var seedProbability = Random.Range(0, 100);
        if (Instance.activeRoom.GetDifficulty() == 0)
        {
            if (seedProbability > Instance.commonSeedProbability * Instance.luckModifier) return;
            if (PermaSeedManager.Instance.HasAllSeeds("Easy")) return;
        }
        else if (Instance.activeRoom.GetDifficulty() == 1)
        {
            if (seedProbability > Instance.rareSeedProbability * Instance.luckModifier) return;
            if (PermaSeedManager.Instance.HasAllSeeds("Medium")) return;
        }
        else if (Instance.activeRoom.GetDifficulty() == 2)
        {
            if (seedProbability > Instance.legendarySeedProbability * Instance.luckModifier) return;
            if (PermaSeedManager.Instance.HasAllSeeds("Hard")) return;
        }

        // Instantiate a perma seed prefab at the given position
        var permaSeed = Instantiate(permaSeedPrefab, position, Quaternion.identity);
        // Set the parent 
        permaSeed.transform.SetParent(Instance.activeRoom.transform);

        Instance.activeRoom.spawnedPermaSeed = true;

        var seedRb = permaSeed.GetComponent<Rigidbody2D>();

        // Apply a force to the essence to make it scatter slightly
        // Apply the force in the opposite direction of the player
        seedRb.AddForce((PlayerController.Instance.transform.position - position).normalized * -10000f,
            ForceMode2D.Impulse);

        // Reset the velocity of the essence after a delay
        StartCoroutine(ResetVelocity(seedRb, false));
    }

    public void DropSpecificPermaSeed(Vector3 position, string seedName)
    {
        // add or subtract 1f from the x and y of the position,
        // this is based on which side of the player is closest to the center
        var playerPosition = PlayerController.Instance.transform.position;
        var roomPosition = Instance.activeRoom.transform.position;
        var x = playerPosition.x - roomPosition.x;
        var y = playerPosition.y - roomPosition.y;

        if (Mathf.Abs(x) > Mathf.Abs(y))
        {
            if (x > 0)
            {
                position.x += 1f;
            }
            else
            {
                position.x -= 1f;
            }
        }
        else
        {
            if (y > 0)
            {
                position.y += 1f;
            }
            else
            {
                position.y -= 1f;
            }
        }

        // Instantiate a perma seed prefab at the given position
        var permaSeed = Instantiate(permaSeedPrefab, position, Quaternion.identity);
        permaSeed.GetComponent<PermaSeedController>().SetPermaSeed(seedName);
        // Set the parent 
        permaSeed.transform.SetParent(Instance.activeRoom.transform);

        var seedRb = permaSeed.GetComponent<Rigidbody2D>();
        seedRb.AddForce((PlayerController.Instance.transform.position - position).normalized * -10000f,
            ForceMode2D.Impulse);
        StartCoroutine(ResetVelocity(seedRb, false));
    }

    private IEnumerator ResetVelocity(Rigidbody2D essence, bool destroy = true)
    {
        yield return new WaitForSeconds(EssenceForceDelay);
        if (essence == null) yield break;
        essence.velocity = Vector2.zero;

        // Start a coroutine that destroys the essence after a delay
        if (destroy)
        {
            StartCoroutine(DestroyEssence(essence.gameObject));
        }
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

    public void AddClover(float multiplier)
    {
        luckModifier *= multiplier;
    }

    public void RemoveClover(float multiplier)
    {
        luckModifier = 1f;
    }

    #endregion

    public void LoadData(GameData data)
    {
        isTutorial = data.IsTutorial;
        HasSeenTrader = data.HasSeenTrader;
        gameFinished = data.gameFinished;
        deeperPortalSpawn = data.deeperPortalSpawn;
        deeperPortalSpawnPrompted = data.deeperPortalSpawnPrompted;
        
        // Populate the fountains activated list
        fountainsActivated = new bool[4];
        for (var i = 0; i < fountainsActivated.Length; i++)
        {
            fountainsActivated[i] = data.fountainActivated[i];
        }
        
        HasSeenCollector = data.HasSeenCollector;
    }

    public void SaveData(GameData data)
    {
        data.IsTutorial = isTutorial;
        data.HasSeenTrader = HasSeenTrader;
        data.gameFinished = gameFinished;
        data.deeperPortalSpawn = deeperPortalSpawn;
        data.deeperPortalSpawnPrompted = deeperPortalSpawnPrompted;
        
        // Populate the fountains activated list
        data.fountainActivated = new bool[4];
        for (var i = 0; i < fountainsActivated.Length; i++)
        {
            data.fountainActivated[i] = fountainsActivated[i];
        }
        data.HasSeenCollector = HasSeenCollector;
    }

    public bool FirstLoad()
    {
        return firstLoad;
    }

    public void SetCursorShoot()
    {
        Cursor.SetCursor(shootTexture, hotSpot, cursorMode);
    }

    public void SetCursorDefault()
    {
        Cursor.SetCursor(null, hotSpot, cursorMode);
    }

    public void SetCursorCannotShoot()
    {
        Cursor.SetCursor(cantShootTexture, hotSpot, cursorMode);
    }
    
    public bool IsActive()
    {
        return gameObject.activeSelf;
    }
}