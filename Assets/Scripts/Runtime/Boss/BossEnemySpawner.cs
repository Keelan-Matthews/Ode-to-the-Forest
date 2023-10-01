using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemySpawner : MonoBehaviour
{
    [SerializeField] private int enemiesToSpawn;
    private bool _justDied;
    
    private void Awake()
    {
        Health.OnPlayerDeath += Health_OnPlayerDeath;
    }
    
    private void OnDestroy()
    {
        Health.OnPlayerDeath -= Health_OnPlayerDeath;
    }
    
    private void Health_OnPlayerDeath()
    {
        _justDied = true;
    }
    
    private IEnumerator SpawnEnemy(Room room, Room.EnemySpawnerData data)
    {
        var enemiesSpawned = 0;
        // While the wave is not over, spawn an enemy and wait for a random interval
        while (enemiesSpawned < enemiesToSpawn)
        {
            // If the player is dead, destroy all enemies in the room and return
            if (_justDied)
            {
                foreach (Transform child in room.transform)
                {
                    Destroy(child.gameObject);
                }
                
                yield break;
            }

            // Get a random position in the room
            var maxIterations = 3;
            Vector2 randomPos;
            do
            {
                randomPos = room.GetRandomPositionInLeftHalfOfRoom();
                maxIterations--;
            }
            while (!Room.ReviewRoomPosition(randomPos, data.spawnerData.itemToSpawn.GetComponent<Collider2D>()) && maxIterations > 0);
            
            if (maxIterations <= 0)
            {
                Debug.LogError("Could not find a valid position to spawn the enemy.");
                continue;
            }
            
            // Spawn the enemy
            var enemy = Instantiate(data.spawnerData.itemToSpawn, randomPos, Quaternion.identity, room.transform);
            enemy.GetComponent<EnemyController>().PlaySpawnAnimation();
            
            enemiesSpawned++;

            yield return new WaitForSeconds(Random.Range(data.spawnerData.minSpawnRate, data.spawnerData.maxSpawnRate + 1));
        }
    }
    
    public void SpawnEnemies()
    {
        var currentRoom = GameManager.Instance.activeRoom;
        var enemySpawners = currentRoom.GetEnemyData();
        StartCoroutine(SpawnEnemy(currentRoom, enemySpawners[0]));
    }
}
