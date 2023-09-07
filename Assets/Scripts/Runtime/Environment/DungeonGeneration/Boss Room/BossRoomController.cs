using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomController : MonoBehaviour
{
    [SerializeField] private GameObject boss;
    [SerializeField] private Transform spawnPoint;
    
    private void Awake()
    {
        TriggerBossBattle.OnStartBossBattle += SpawnBoss;
    }
    
    private void SpawnBoss()
    {
        Instantiate(boss, spawnPoint.position, Quaternion.identity);
    }
}