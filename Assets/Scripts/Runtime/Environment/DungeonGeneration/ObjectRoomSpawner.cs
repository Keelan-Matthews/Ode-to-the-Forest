using System;
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
            GenerateObjects(rs);
        }
    }
    private void GenerateObjects(RandomSpawner data)
    {
        // Get the number of objects to spawn from the spawner data
        var numObjects = Random.Range(data.spawnerData.minSpawnRate, data.spawnerData.maxSpawnRate + 1);
        
        // For each object to spawn, get a random position from the grid and spawn the object
        for (var i = 0; i < numObjects; i++)
        {
            var randomPos = grid.gridPositions[Random.Range(0, grid.gridPositions.Count)];
            var obj = Instantiate(data.spawnerData.itemToSpawn, randomPos, Quaternion.identity, transform);
            obj.name = data.name;
            grid.gridPositions.Remove(randomPos);
            
            // Remove the surrounding grid positions so that objects don't spawn too close to each other
            RemoveSurroundingGridPositions(randomPos);
        }
    }
    
    private void RemoveSurroundingGridPositions(Vector2 pos)
    {
        // Remove the surrounding grid positions so that objects don't spawn too close to each other
        for (var i = 0; i < 2; i++)
        {
            grid.gridPositions.Remove(new Vector2(pos.x + i, pos.y));
            grid.gridPositions.Remove(new Vector2(pos.x - i, pos.y));
            grid.gridPositions.Remove(new Vector2(pos.x, pos.y + i));
            grid.gridPositions.Remove(new Vector2(pos.x, pos.y - i));
        }
    }
}