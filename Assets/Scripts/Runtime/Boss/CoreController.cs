using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreController : MonoBehaviour
{
    [SerializeField] private int hitPoints;
    private int _currentHitPoints;
    public bool coreDestroyed;
    public bool canTakeDamage;
    
    public static event Action OnCoreDestroyed;
    public static event Action<int> OnCoreHit;
    
    private void Awake()
    {
        _currentHitPoints = hitPoints;
    }
    
    public void TakeDamage(int damage)
    {
        _currentHitPoints -= damage;
        OnCoreHit?.Invoke(damage);
        if (_currentHitPoints <= 0)
        {
            coreDestroyed = true;
            OnCoreDestroyed?.Invoke();
        }
    }
    
    public int GetHitPoints()
    {
        return hitPoints;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Bullet") || coreDestroyed || !canTakeDamage) return;
        // Make sure it is not an enemy bullet
        if (other.GetComponent<BulletController>().isEnemyBullet) return;
        var damage = PlayerController.Instance.FireDamage;
        TakeDamage(damage);
        AudioManager.PlaySound(AudioManager.Sound.EnemyHit, transform.position);
    }
}
