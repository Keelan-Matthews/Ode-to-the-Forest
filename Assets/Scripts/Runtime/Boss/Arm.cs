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
    public int armHitPoints = 4;
    private int _currentHealth;
    public int stateNumber;
    
    [SerializeField] private RuntimeAnimatorController[] armStates;

    public static event Action OnArmDestroyed;
    
    private void Awake()
    {
        _currentHealth = armHitPoints;
    }
    
    public void TakeDamage(int damage)
    {
        if (!isExposed) return;
        bossController.UpdateHealthBar(damage);
        _currentHealth -= damage;
        
        // Update the arm states for slight health, half health, and quarter health
        if (_currentHealth <= armHitPoints * 0.75f && _currentHealth > armHitPoints * 0.5f)
        {
            stateNumber = 1;
            GetComponent<Animator>().runtimeAnimatorController = armStates[stateNumber];
        }
        else if (_currentHealth <= armHitPoints * 0.5f && _currentHealth > armHitPoints * 0.25f)
        {
            stateNumber = 2;
            GetComponent<Animator>().runtimeAnimatorController = armStates[stateNumber];
        }
        else if (_currentHealth <= armHitPoints * 0.25f)
        {
            stateNumber = 3;
            GetComponent<Animator>().runtimeAnimatorController = armStates[stateNumber];
        }
        
        if (_currentHealth <= 0)
        {
            OnArmDestroyed?.Invoke();
            fireArmsController.RemoveArm(index);
            Destroy(gameObject);
        }
    }
}
