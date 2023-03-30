using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectRoomSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct RandomSpawner
    {
        public string name;
        public SpawnerData spawnerData;
    }

    public GridController grid;
    public RandomSpawner[] randomSpawners;

    public void InitializeObjectSpawning()
    {
        // Spawn the objects based on the random spawner data
        foreach (var rs in randomSpawners)
        {
            SpawnObjects(rs);
        }
    }

    private void SpawnObjects(RandomSpawner data)
    {
        // Get the difficulty of the room this spawner is in
        var difficulty = grid.room.GetDifficulty();
        // Get the duration of the wave
        var waveDuration = GetWaveDuration(difficulty);

        // Spawn the objects in the room for the duration of the wave with the spawn rate as an interval as a coroutine
        // in a random grid position
        StartCoroutine(SpawnObjectsInWave(data.spawnerData, waveDuration));
    }
    
    private IEnumerator SpawnObjectsInWave(SpawnerData data, int waveDuration)
    {
        // Get the current time
        var startTime = Time.time;
        // Get the end time
        var endTime = startTime + waveDuration;
        // Get the current time
        var currentTime = Time.time;
        
        // While the current time is less than the end time
        while (currentTime < endTime)
        {
            // Get a random spawn rate for the wave every second using spawner data
            var spawnRate = Random.Range(data.minSpawnRate, data.maxSpawnRate);
            // Get a random position from the grid
            var randomPos = Random.Range(0, grid.gridPositions.Count - 1);
            // Spawn the object at the random position
            Instantiate(data.itemToSpawn, grid.gridPositions[randomPos], Quaternion.identity, transform);
            // Wait for the spawn rate
            yield return new WaitForSeconds(spawnRate);
            // Get the current time
            currentTime = Time.time;
        }
    }
    
    private int GetWaveDuration(int difficulty)
    {
        var waveDuration = difficulty switch
        {
            1 => 10,
            2 => 20,
            3 => 30,
            _ => 10
        };

        return waveDuration;
    }
}
