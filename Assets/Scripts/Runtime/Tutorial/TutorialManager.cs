using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TutorialManager : MonoBehaviour
{
    public GameObject dialogueBox;
    public DialogueController dialogueController;
    public GameObject permaSeedPrefab;
    public Canvas uICanvas;

    public static TutorialManager Instance;
    private Vector2 _lastEnemyPosition;
    private bool setInvincible;
    private bool droppedMinimapSeed;
    
    private void Awake()
    {
        Instance = this;
    }
    
    // Start is called before the first frame update
    private void Start()
    {
        PlayerController.Instance.SetInvincible(true);
        dialogueController = dialogueBox.GetComponent<DialogueController>();
        
        // Subscribe to the OnRoomClear event
        RoomController.OnRoomCleared += RoomController_OnRoomCleared;
        
        GameManager.Instance.isTutorial = true;
        
        uICanvas.enabled = false;
        
        // Subscribe to the OnPlayerDeath event
        // Health.OnPlayerDeath += Health_OnPlayerDeath;
    }

    // private void Health_OnPlayerDeath()
    // {
    //     // Spawn the player in the previous room
    //     // Reset the player's health
    //     // Reset the wave room
    // }

    private void RoomController_OnRoomCleared(Room obj)
    {
        // If the room name is "WaveRoom" then spawn a minimap seed
        if (obj.name == "WaveRoom" && !droppedMinimapSeed)
        {
            DropMinimapSeed();
            droppedMinimapSeed = true;
            ResumeTutorial();
        }
    }

    public void StartTutorial()
    {
        dialogueBox.SetActive(true);
        dialogueController.StartDialogue();
    }
    
    public void ResumeTutorial()
    {
        dialogueBox.SetActive(true);
        dialogueController.ResumeDialogue();
    }

    private void Update()
    {
        if (!GameManager.Instance.activeRoom || GameManager.Instance.activeRoom.name != "WaveRoom") return;

        if (!setInvincible)
        {
            uICanvas.enabled = true;
            setInvincible = true;
        }
        
        // Keep track of how many enemies are in the room
        var currentRoom = GameManager.Instance.activeRoom;
        var enemyCount = currentRoom.GetActiveEnemyCount();
        
        // If there is one enemy left in the room, store its position
        if (enemyCount == 1)
        {
            _lastEnemyPosition = currentRoom.GetEnemies()[0].transform.position;
        }
    }

    public void DropMinimapSeed()
    {
        // Update the position to be slightly off the player
        _lastEnemyPosition += new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f));
        var permaSeed = Instantiate(permaSeedPrefab, _lastEnemyPosition, Quaternion.identity);
       permaSeed.GetComponent<PermaSeedController>().SetPermaSeed("Minimap");
        // Set the parent 
        permaSeed.transform.SetParent(transform);

        var seedRb = permaSeed.GetComponent<Rigidbody2D>();
        
        // Apply a force to the essence to make it scatter slightly
        seedRb.AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * -15000f, ForceMode2D.Impulse);
        
        // Reset the velocity of the essence after a delay
        StartCoroutine(ResetVelocity(seedRb));
        
        PlayerController.Instance.SetInvincible(false);
    }
    
    private IEnumerator ResetVelocity(Rigidbody2D essence)
    {
        yield return new WaitForSeconds(0.3f);
        if (essence == null) yield break;
        essence.velocity = Vector2.zero;
    }
}
