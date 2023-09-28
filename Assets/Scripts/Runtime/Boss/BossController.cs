using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public static BossController Instance;
    public int coresDestroyed = 0;
    public int armsDestroyed = 0;
    public bool isDead;
    public bool isEnraged;

    private BossHealthBar bossHealthBar;
    private int maxHealth;
    private int _currentHealth;

    private void Awake()
    {
        Instance = this;
        // Find tag BossHealthBar in canvas
        bossHealthBar = GameObject.FindGameObjectWithTag("BossHealthBar").GetComponent<BossHealthBar>();
        // Enable the slider and image in its children
        bossHealthBar.transform.GetChild(0).gameObject.SetActive(true);
        bossHealthBar.transform.GetChild(1).gameObject.SetActive(true);
        
        CoreController.OnCoreDestroyed += CheckCores;
        Arm.OnArmDestroyed += CheckArms;
        CoreController.OnCoreHit += UpdateHealthBar;

        var coreHitPoints = BossRoomController.Instance.GetCoreHitPoints();
        var armHitPoints = GetComponent<FireArmsController>().GetTotalHealth();

        maxHealth = armHitPoints + coreHitPoints;
        _currentHealth = maxHealth;
        
        bossHealthBar.SetMaxHealth(maxHealth);
    }

    public void UpdateHealthBar(int damage)
    {
        _currentHealth -= damage;
        bossHealthBar.SetHealth(_currentHealth);
        if (_currentHealth <= 0)
        {
            isDead = true;
        }
    }

    private void CheckCores()
    {
        coresDestroyed++;
    }
    
    private void CheckArms()
    {
        armsDestroyed++;
    }
    
    public void DropSeedOfLife()
    {
        GameManager.Instance.DropSpecificPermaSeed(transform.position, "Seed Of Life");
        GameManager.Instance.DropEssence(20, transform.position, false);
        
        // Play the wave end sound
        AudioManager.PlaySound(AudioManager.Sound.WaveEnd, transform.position);
        
        // Get the active room
        var activeRoom = GameManager.Instance.activeRoom;
        activeRoom.OnWaveEnd(true);
    }
}