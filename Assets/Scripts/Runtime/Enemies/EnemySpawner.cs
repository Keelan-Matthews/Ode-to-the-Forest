using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float waveDuration = 10f;
    public static EnemySpawner Instance;
    
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
        
        // Get the start time
        room.waveStartTime = Time.time;
        
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
    }
}
