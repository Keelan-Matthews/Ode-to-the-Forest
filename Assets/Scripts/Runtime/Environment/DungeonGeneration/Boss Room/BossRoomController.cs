using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomController : MonoBehaviour
{
    [SerializeField] private GameObject boss;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private List<CoreController> cores;

    private void Awake()
    {
        TriggerBossBattle.OnStartBossBattle += SpawnBoss;
    }
    
    private void SpawnBoss()
    {
        Instantiate(boss, spawnPoint.position, Quaternion.identity);
    }
    
    public void ExposeCores()
    {
        foreach (var core in cores)
        {
            core.canTakeDamage = true;
        }
    }
    
    public void HideCores()
    {
        foreach (var core in cores)
        {
            core.canTakeDamage = false;
        }
    }
}
