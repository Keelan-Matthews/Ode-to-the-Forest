using UnityEditor.U2D.Path;
using UnityEngine;

[CreateAssetMenu(fileName = "Spawner.asset", menuName = "Spawners/Spawner", order = 0)]
public class SpawnerData : ScriptableObject
{
    public GameObject itemToSpawn;
    public int minSpawnRate, maxSpawnRate;
}
