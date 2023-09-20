using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public static BossController Instance;
    public int coresDestroyed;
    public bool isDead;

    private BossHealthBar bossHealthBar;
    [SerializeField] private int bossHitPoints;
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
        CoreController.OnCoreHit += UpdateHealthBar;

        var coreHitPoints = BossRoomController.Instance.GetCoreHitPoints();

        maxHealth = bossHitPoints + coreHitPoints;
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
}