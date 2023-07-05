using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    public PurificationMeter purificationMeter;
    private bool _isSpawning;
    private Room _currentRoom;
    private bool _isPurifying;
    private bool _playerIsDead;

    private void Awake()
    {
        // Subscribe to the OnStartWave event
        GameManager.OnStartWave += GameManager_OnStartWave;
        Health.OnPlayerDeath += Health_OnPlayerDeath;
    }

    // Unsubscribe on destroy
    private void OnDestroy()
    {
        GameManager.OnStartWave -= GameManager_OnStartWave;
        Health.OnPlayerDeath -= Health_OnPlayerDeath;
    }
    
    private void Health_OnPlayerDeath()
    {
        _playerIsDead = true;
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
        
        _currentRoom = room;
        
        _isSpawning = true;
        _isPurifying = true;
        
        // Get the start time
        _currentRoom.waveStartTime = Time.time;

        // Initialize the purification meter
        purificationMeter.SetMaxPurification(_currentRoom.waveDuration);
        // Enable the purification meter
        purificationMeter.gameObject.SetActive(true);
        
        // PLay the wave start sound
        AudioManager.PlaySound(AudioManager.Sound.WaveStart, transform.position);

        // Get the spawnable enemies for this room type
        var enemySpawners = _currentRoom.GetEnemyData();

        // For every type of spawner in the room type (easy, medium, etc) spawn a random enemy
        // from the spawner and wait for a random interval as specified in the spawner.
        // Do so while the wave is not over.
        foreach (var enemySpawner in enemySpawners)
        {
            // Only start the coroutine for the active room's spawner
            StartCoroutine(SpawnEnemy(_currentRoom, enemySpawner));
        }
    }

    private IEnumerator SpawnEnemy(Room room, Room.EnemySpawnerData data)
    {
        // While the wave is not over, spawn an enemy and wait for a random interval
        while (Time.time < room.waveStartTime + room.waveDuration)
        {
            // If the player is dead, destroy all enemies in the room and return
            if (_playerIsDead)
            {
                foreach (Transform child in room.transform)
                {
                    Destroy(child.gameObject);
                }
                yield break;
            }
            
            // Get a random position in the room
            var randomPos = room.GetRandomPositionInRoom();
            
            // Review the room position to see if there is enough space to spawn an enemy
            if (!Room.ReviewRoomPosition(randomPos, data.spawnerData.itemToSpawn.GetComponent<Collider2D>())) continue;
            // Spawn the enemy
            var enemy = Instantiate(data.spawnerData.itemToSpawn, randomPos, Quaternion.identity, room.transform);
            enemy.GetComponent<EnemyController>().PlaySpawnAnimation();
            
            // Set the enemy difficulty through EnemyController
            // enemy.GetComponent<EnemyController>().SetDifficulty(room.difficulty);
            
            // Wait for a random interval if the wave is not over and there are still enemies in the room
            if (Time.time < room.waveStartTime + room.waveDuration && room.GetActiveEnemyCount() > 0)
            {
                yield return new WaitForSeconds(Random.Range(data.spawnerData.minSpawnRate, data.spawnerData.maxSpawnRate + 1));
            }
        }
        
        _isPurifying = false;
        
        // If the timer is up and there are still enemies in the room, wait until they have been killed
        while (room.GetActiveEnemyCount() > 0)
        {
            yield return null;
        }
        
        purificationMeter.SetPurification(_currentRoom.waveDuration);
        
        // Play the wave end sound
        AudioManager.PlaySound(AudioManager.Sound.WaveEnd, transform.position);
        
        // Wait for 1 second then disable the purification meter
        yield return new WaitForSeconds(0.3f);

        // Disable the purification meter
        purificationMeter.gameObject.SetActive(false);
        
        // Set the isSpawning flag to false
        _isSpawning = false;
        
        // When the wave is over, call the OnWaveEnd event
        room.OnWaveEnd();
        
        // Set player controllers inSunlight to true
        PlayerController.Instance.inSunlight = true;
    }

    private void Update()
    {
        const float ballMeterThreshold = 0.937f;
        // If the wave is over, or there is no active room, return
        if (!_isSpawning || !_isPurifying) return;
        // Update the purification meter
        if (purificationMeter.GetPurification() < ballMeterThreshold * _currentRoom.waveDuration)
        {
            purificationMeter.SetPurification(Time.time - _currentRoom.waveStartTime);
        }
    }
}
