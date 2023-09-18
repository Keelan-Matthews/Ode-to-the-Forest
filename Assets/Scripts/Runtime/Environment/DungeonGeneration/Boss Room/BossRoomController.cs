using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BossRoomController : MonoBehaviour
{
    [SerializeField] private GameObject boss;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private List<CoreController> cores;
    
    [SerializeField] private SporadicSunlightController sporadicSunlight;
    [SerializeField] private SunlightController sunlightController;

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
    
    public void ActivateSporadicSunlight()
    {
        sunlightController.Dim();
        sunlightController.GetComponent<BoxCollider2D>().enabled = false;
        sunlightController.enabled = false;
        sporadicSunlight.Spawn();
    }
}
