using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectRoomSpawner : MonoBehaviour
{
    [Serializable]
    public struct RandomSpawner
    {
        public string name;
        public SpawnerData spawnerData;
    }

    public GridController grid;
    public RandomSpawner[] randomSpawners;

    public void InitializeObjectSpawning()
    {
        // Spawn the objects for each type of spawner (easy, medium, hard etc.)
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
        // var randomGridPosition = grid.gridPositions[Random.Range(0, grid.gridPositions.Count - 1)];
        // Instantiate(data.spawnerData.itemToSpawn, randomGridPosition, Quaternion.identity, transform);

        // Spawn the objects in the room for the duration of the wave with the spawn rate as an interval
        // in a random grid position
        StartCoroutine(SpawnObjectsInWave(data.spawnerData, waveDuration));
    }
    
    private IEnumerator SpawnObjectsInWave(SpawnerData data, int waveDuration)
    {
        // Spawn the objects in the room for the duration of the wave with the spawn rate as an interval
        // in a random grid position
        for (var i = 0; i < waveDuration; i++)
        {
            var randomGridPosition = grid.gridPositions[Random.Range(0, grid.gridPositions.Count - 1)];
            Instantiate(data.itemToSpawn, randomGridPosition, Quaternion.identity, transform);
            //Get random spawn rate
            var randomSpawnRate = Random.Range(data.minSpawnRate, data.maxSpawnRate + 1);
            yield return new WaitForSeconds(randomSpawnRate);
        }
    }


    private static int GetWaveDuration(int difficulty)
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