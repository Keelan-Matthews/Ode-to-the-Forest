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
    
    [SerializeField] private RuntimeAnimatorController[] armStatesLeft;
    [SerializeField] private RuntimeAnimatorController[] armStatesRight;
    [SerializeField] private AimController aimController;

    public static event Action OnArmDestroyed;
    
    private void Awake()
    {
        _currentHealth = armHitPoints;
    }
    
    public void TakeDamage(int damage)
    {
        if (!isExposed) return;
        AudioManager.PlaySound(AudioManager.Sound.EnemyHit, transform.position);
        bossController.UpdateHealthBar(damage);
        _currentHealth -= damage;
        
        // Update the arm states for slight health, half health, and quarter health
        if (_currentHealth <= armHitPoints * 0.75f && _currentHealth > armHitPoints * 0.5f)
        {
            stateNumber = 1;
        }
        else if (_currentHealth <= armHitPoints * 0.5f && _currentHealth > armHitPoints * 0.25f)
        {
            stateNumber = 2;
        }
        else if (_currentHealth <= armHitPoints * 0.25f)
        {
            stateNumber = 3;
        }
        
        aimController.SetArmState(stateNumber, true);

        if (_currentHealth <= 0)
        {
            OnArmDestroyed?.Invoke();
            fireArmsController.RemoveArm(index);
            Destroy(gameObject);
        }
    }

    public void UpdateState()
    {
        // Update left arm if index is 1
        if (index == 1)
        {
            GetComponent<Animator>().runtimeAnimatorController = armStatesLeft[stateNumber];
        }
        // Update right arm if index is 0
        else if (index == 0)
        {
            GetComponent<Animator>().runtimeAnimatorController = armStatesRight[stateNumber];
        }
    }
}
