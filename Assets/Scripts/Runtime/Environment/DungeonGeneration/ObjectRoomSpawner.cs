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
    [SerializeField] private GameObject fireflyParticles;
    
    private void Start()
    {
        // Get the NavMesh gameobject in the scene
        _surface2D = FindObjectOfType<NavMeshSurface>();
        _surface2D.BuildNavMeshAsync();
    }

    public void InitializeObjectSpawning()
    {
        if (randomSpawners.Length == 0) return;

        const int easyMin = 3;
        const int easyMax = 3;
        const int mediumMin = 2;
        const int mediumMax = 2;
        const int hardMin = 1;
        const int hardMax = 2;

        var totalSpawned = 0;

        switch (spawnerType)
        {
            case SpawnerType.Easy:
                totalSpawned = 0;
                var easySpawnRate = Random.Range(easyMin, easyMax + 1);
                int attempts = 0;
                while (totalSpawned < easySpawnRate && attempts < easySpawnRate * 10)
                {
                    var randomSpawner = randomSpawners[Random.Range(0, randomSpawners.Length)];
                    totalSpawned += GenerateObjects(randomSpawner);
                    attempts++;
                }
                break;

            case SpawnerType.Medium:
                totalSpawned = 0;
                var mediumSpawnRate = Random.Range(mediumMin, mediumMax + 1);
                attempts = 0;
                while (totalSpawned < mediumSpawnRate && attempts < mediumSpawnRate * 10)
                {
                    var randomSpawner = randomSpawners[Random.Range(0, randomSpawners.Length)];
                    totalSpawned += GenerateObjects(randomSpawner);
                    attempts++;
                }
                break;

            case SpawnerType.Hard:
                totalSpawned = 0;
                var hardSpawnRate = Random.Range(hardMin, hardMax + 1);
                attempts = 0;
                while (totalSpawned < hardSpawnRate && attempts < hardSpawnRate * 10)
                {
                    var randomSpawner = randomSpawners[Random.Range(0, randomSpawners.Length)];
                    totalSpawned += GenerateObjects(randomSpawner);
                    attempts++;
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
        if (grid.gridPositions.Count == 0) return 0;
        
        // Get the number of objects to spawn from the spawner data
        var numObjects = Random.Range(data.spawnerData.minSpawnRate, data.spawnerData.maxSpawnRate + 1);
        
        // For each object to spawn, get a random position from the grid and spawn the object
        for (var i = 0; i < numObjects; i++)
        {
            var index = Random.Range(0, grid.gridPositions.Count);
            if (index >= grid.gridPositions.Count) continue;
            var randomPos = grid.gridPositions[index];
            var obj = Instantiate(data.spawnerData.itemToSpawn, randomPos, Quaternion.identity, transform);
            grid.gridPositions.Remove(randomPos);
            
            // Remove the surrounding grid positions so that objects don't spawn too close to each other
            RemoveSurroundingGridPositions(randomPos);
        }
        
        return (int) numObjects;
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
    
    public GameObject[] GetObstacles()
    {
        // Get all the obstacles in the room
        var obstacles = new GameObject[transform.childCount];
        for (var i = 0; i < transform.childCount; i++)
        {
            obstacles[i] = transform.GetChild(i).gameObject;
        }

        return obstacles;
    }
    
    public void AddFireflyParticles()
    {
        // Add firefly particles to the room
        var particles = Instantiate(fireflyParticles, transform.position, Quaternion.identity, transform);
        
        // Place them in the same palce as a random obstacle
        var obstacles = GetObstacles();
        var randomObstacle = obstacles[Random.Range(0, obstacles.Length)];
        particles.transform.position = randomObstacle.transform.position;
    }
}