using System;
using UnityEngine;
using Random = UnityEngine.Random;
using NavMeshPlus.Components;

public class ObjectRoomSpawner : MonoBehaviour
{
    [Serializable]
    public struct RandomSpawner
    {
        public SpawnerData spawnerData;
    }
    
    public enum SpawnerType
    {
        Easy,
        Medium,
        Hard
    }

    public GridController grid;
    public SpawnerType spawnerType;
    public RandomSpawner[] randomSpawners;
    private NavMeshSurface _surface2D;
    
    private void Start()
    {
        // Get the NavMesh gameobject in the scene
        _surface2D = FindObjectOfType<NavMeshSurface>();
        _surface2D.BuildNavMeshAsync();
    }

    public void InitializeObjectSpawning()
    {
        const int easyMin = 0;
        const int easyMax = 3;
        const int mediumMin = 2;
        const int mediumMax = 5;
        const int hardMin = 4;
        const int hardMax = 5;
        // Spawn the objects for each type of spawner (easy, medium, hard etc.)
        // by picking a number of random spawners in the list based on the spawner type
        var totalSpawned = 0;
        switch (spawnerType)
        {
            case SpawnerType.Easy:
                // Pick a random number of spawners from the list,
                // ensuring that their max spawn rate is less than or equal to 2
                totalSpawned = 0;
                var easySpawnRate = Random.Range(easyMin, easyMax + 1);
                while (totalSpawned < easySpawnRate)
                {
                    var randomSpawner = randomSpawners[Random.Range(0, randomSpawners.Length)];
                    totalSpawned += GenerateObjects(randomSpawner);
                }
                break;
            case SpawnerType.Medium:
                // Pick a random number of spawners from the list,
                // ensuring that their max spawn rate is between 2 and 5
                totalSpawned = 0;
                var mediumSpawnRate = Random.Range(mediumMin, mediumMax + 1);
                while (totalSpawned < mediumSpawnRate)
                {
                    var randomSpawner = randomSpawners[Random.Range(0, randomSpawners.Length)];
                    totalSpawned += GenerateObjects(randomSpawner);
                }
                break;
            case SpawnerType.Hard:
                // Pick a random number of spawners from the list,
                // ensuring that their max spawn rate is greater than or equal to 4
                totalSpawned = 0;
                var hardSpawnRate = Random.Range(hardMin, hardMax + 1);
                while (totalSpawned < hardSpawnRate)
                {
                    var randomSpawner = randomSpawners[Random.Range(0, randomSpawners.Length)];
                    totalSpawned += GenerateObjects(randomSpawner);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (_surface2D != null)
        {
            _surface2D.UpdateNavMesh(_surface2D.navMeshData);
        }
    }
    private int GenerateObjects(RandomSpawner data)
    {
        // Get the number of objects to spawn from the spawner data
        var numObjects = Random.Range(data.spawnerData.minSpawnRate, data.spawnerData.maxSpawnRate + 1);
        
        // For each object to spawn, get a random position from the grid and spawn the object
        for (var i = 0; i < numObjects; i++)
        {
            var randomPos = grid.gridPositions[Random.Range(0, grid.gridPositions.Count)];
            var obj = Instantiate(data.spawnerData.itemToSpawn, randomPos, Quaternion.identity, transform);
            grid.gridPositions.Remove(randomPos);
            
            // Remove the surrounding grid positions so that objects don't spawn too close to each other
            RemoveSurroundingGridPositions(randomPos);
        }
        
        return numObjects;
    }
    
    private void RemoveSurroundingGridPositions(Vector2 pos)
    {
        // Remove the surrounding grid positions so that objects don't spawn too close to each other
        for (var i = 0; i < 4; i++)
        {
            grid.gridPositions.Remove(new Vector2(pos.x + i, pos.y));
            grid.gridPositions.Remove(new Vector2(pos.x - i, pos.y));
            grid.gridPositions.Remove(new Vector2(pos.x, pos.y + i));
            grid.gridPositions.Remove(new Vector2(pos.x, pos.y - i));
        }
    }
}