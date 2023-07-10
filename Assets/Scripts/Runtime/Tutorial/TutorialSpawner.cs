using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSpawner : MonoBehaviour
{
    public GameObject firstEnemy;
    private bool _enemySpawned;
    private bool tutorialStarted;
    public Room room;
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Spawn the first enemy
        if (other.CompareTag("Player") && !_enemySpawned)
        {
            TutorialManager.Instance.ResumeTutorial();
            tutorialStarted = true;
        }
    }

    private void Update()
    {
        // If the enemy has been spawned and now there are no enemies left
        // trigger the OnWaveEnd event
        if (_enemySpawned && room.GetActiveEnemyCount() == 0)
        {
            room.OnWaveEnd();
        }
        
        // If the tutorial has not started yet, return
        // If the room has already spawned an enemy, return
        // If there is active dialogue, return
        if (!tutorialStarted || _enemySpawned || GameManager.Instance.activeDialogue) return;
        
        // Instantiate the enemy
        // Get a random position in the room
        var randomPos = room.GetRandomPositionInRoom();
            
        // Review the room position to see if there is enough space to spawn an enemy
        if (!Room.ReviewRoomPosition(randomPos, firstEnemy.GetComponent<Collider2D>())) return;
        // Spawn the enemy
        var enemy = Instantiate(firstEnemy, randomPos, Quaternion.identity, room.transform);
        enemy.GetComponent<EnemyController>().PlaySpawnAnimation();
            
        // Set the enemy difficulty through EnemyController
        // enemy.GetComponent<EnemyController>().SetDifficulty(room.difficulty);
        _enemySpawned = true;
        room.hasWave = true;
    }
}
