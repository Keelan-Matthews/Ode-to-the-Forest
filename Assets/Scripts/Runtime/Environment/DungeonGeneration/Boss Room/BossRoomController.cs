using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BossRoomController : MonoBehaviour
{
    public static BossRoomController Instance;
    [SerializeField] private GameObject boss;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private List<CoreController> cores;

    [SerializeField] private SporadicSunlightController sporadicSunlight;
    [SerializeField] private SunlightController sunlightController;
    [SerializeField] private AudioSource bossMusic;

    private void Awake()
    {
        Instance = this;

        TriggerBossBattle.OnStartBossBattle += SpawnBoss;
    }
    
    private void SpawnBoss()
    {
        Instantiate(boss, spawnPoint.position, Quaternion.identity);
        
        CameraController.Instance.GetComponentInParent<CameraShake>().ShakeCamera(1.5f);
        bossMusic.Play();
    }
    
    public void FadeOutMusic()
    {
        StartCoroutine(FadeOutMusicC());
    }
    
    private IEnumerator FadeOutMusicC()
    {
        var startVolume = bossMusic.volume;
        while (bossMusic.volume > 0)
        {
            bossMusic.volume -= startVolume * Time.deltaTime / 2;
            yield return null;
        }
        bossMusic.Stop();
        bossMusic.volume = startVolume;
    }
    
    public int GetCoreHitPoints()
    {
        var hitPoints = 0;
        foreach (var core in cores)
        {
            hitPoints += core.GetHitPoints();
        }
        return hitPoints;
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
