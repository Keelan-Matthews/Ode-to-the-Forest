using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;
    public PurificationMeter purificationMeter;
    private bool _isSpawning = false;
    
    private void Awake()
    {
        Instance = this;
        
        // Subscribe to the OnStartWave event
        GameManager.OnStartWave += GameManager_OnStartWave;
    }

    /*
     * This method listens to when a wave is started and checks to see if it should be started in the current
     * room. If so, it utilizes the available enemy prefabs specific to the room type and spawns them in the
     * room for the wave duration.
     */
    private void GameManager_OnStartWave(Room room)
    {
        // If the room is not the active room, return
        if (room != GameManager.Instance.activeRoom) return;
        
        _isSpawning = true;
        
        // Get the start time
        room.waveStartTime = Time.time;
        
        // Initialize the purification meter
        purificationMeter.SetMaxPurification(room.waveDuration);
        // Enable the purification meter
        purificationMeter.gameObject.SetActive(true);

        // Get the spawnable enemies for this room type
        var enemySpawners = room.GetEnemyData();

        // For every type of spawner in the room type (easy, medium, etc) spawn a random enemy
        // from the spawner and wait for a random interval as specified in the spawner.
        // Do so while the wave is not over.
        foreach (var enemySpawner in enemySpawners)
        {
            // Only start the coroutine for the active room's spawner
            StartCoroutine(SpawnEnemy(room, enemySpawner));
        }
    }

    private IEnumerator SpawnEnemy(Room room, Room.EnemySpawnerData data)
    {
        // Get the wave duration from the room
        var waveDuration = room.waveDuration;
        
        // While the wave is not over, spawn an enemy and wait for a random interval
        while (Time.time < room.waveStartTime + waveDuration)
        {
            // Get a random position in the room
            var randomPos = room.GetRandomPositionInRoom();
            // Spawn the enemy
            var enemy = Instantiate(data.spawnerData.itemToSpawn, randomPos, Quaternion.identity, transform);
            // Wait for a random interval
            yield return new WaitForSeconds(Random.Range(data.spawnerData.minSpawnRate, data.spawnerData.maxSpawnRate + 1));
        }
        
        // When the wave is over, call the OnWaveEnd event
        room.OnWaveEnd();
        // Disable the purification meter
        purificationMeter.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        // If there is no active room, return
        if (!_isSpawning) return;
        
        // Get the wave start time and duration from the active room
        var waveStartTime = GameManager.Instance.activeRoom.waveStartTime;
        var waveDuration = GameManager.Instance.activeRoom.waveDuration;
        
        // If the wave is over, return
        if (Time.time > waveStartTime + waveDuration) return;
        
        // Update the purification meter
        purificationMeter.SetPurification(Time.time - waveStartTime);
    }
}
