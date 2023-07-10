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

    public static TutorialManager Instance;
    private Vector2 _lastEnemyPosition;
    
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
    }

    private void RoomController_OnRoomCleared(Room obj)
    {
        // If the room name is "WaveRoom" then spawn a minimap seed
        if (obj.name == "WaveRoom")
        {
            DropMinimapSeed();
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
        if (GameManager.Instance.activeRoom.name != "WaveRoom") return;
        
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
        var permaSeed = Instantiate(permaSeedPrefab, _lastEnemyPosition, Quaternion.identity);
       permaSeed.GetComponent<PermaSeedController>().SetPermaSeed("MinimapSeed");
        // Set the parent 
        permaSeed.transform.SetParent(transform);

        var seedRb = permaSeed.GetComponent<Rigidbody2D>();
        
        // Apply a force to the essence to make it scatter slightly
        seedRb.AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * 5f, ForceMode2D.Impulse);
        
        // Reset the velocity of the essence after a delay
        StartCoroutine(ResetVelocity(seedRb));
    }
    
    private IEnumerator ResetVelocity(Rigidbody2D essence)
    {
        yield return new WaitForSeconds(0.3f);
        essence.velocity = Vector2.zero;
    }
}
