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
    [SerializeField] private GameObject bossPortal;

    private void Awake()
    {
        Instance = this;

        TriggerBossBattle.OnStartBossBattle += SpawnBoss;
    }
    
    public void SpawnPortal()
    {
        bossPortal.SetActive(true);
        bossPortal.GetComponent<BossPortalController>().Spawn();
    }
    
    private void SpawnBoss()
    {
        var spawnPosition = new Vector3(1.96f, 0.54f, 0);
        // Get these coordinates in relation to the active room
        spawnPosition += GameManager.Instance.activeRoom.transform.position;

        Instantiate(boss, spawnPosition, Quaternion.identity);
        
        
        CameraController.Instance.GetComponentInParent<CameraShake>().ShakeCamera(1.5f);
        bossMusic.Play();
        AudioManager.PlaySound(AudioManager.Sound.BossRoomStart, transform.position);
        
        // Remove any seed from the player's inventory
        if (PermaSeedManager.Instance.HasSeed())
        {
            PermaSeedManager.Instance.RemoveStoredPermaSeed();
        }
    }
    
    public void FadeOutMusic()
    {
        StartCoroutine(FadeOutMusicC());
    }
    
    private IEnumerator FadeOutMusicC()
    {
        var startVolume = bossMusic.volume;
        var duration = 4f;
        while (bossMusic.volume > 0)
        {
            bossMusic.volume -= startVolume * Time.deltaTime / duration;
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
            if (core.coreDestroyed) continue;
            core.canTakeDamage = true;
            core.ExposeCore();
        }
    }
    
    public void HideCores()
    {
        foreach (var core in cores)
        {
            core.canTakeDamage = false;
            if (core.coreDestroyed) continue;
            core.ProtectCore();
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
