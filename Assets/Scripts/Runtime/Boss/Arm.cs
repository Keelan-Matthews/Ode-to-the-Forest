using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arm : MonoBehaviour
{
    public int index;
    [SerializeField] private BossController bossController;
    [SerializeField] private FireArmsController fireArmsController;
    public bool isExposed;
    public int armHitPoints = 10;
    private int _currentHealth;

    public static event Action OnArmDestroyed;
    
    public void TakeDamage(int damage)
    {
        if (!isExposed) return;
        bossController.UpdateHealthBar(damage);
        _currentHealth -= damage;
        
        if (_currentHealth <= 0)
        {
            OnArmDestroyed?.Invoke();
            fireArmsController.RemoveArm(index);
            Destroy(gameObject);
        }
    }
}
