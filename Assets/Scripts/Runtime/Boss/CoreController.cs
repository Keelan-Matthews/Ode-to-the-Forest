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
    [SerializeField] private RuntimeAnimatorController[] coreStates;
    private int _stateNumber;
    private static readonly int QuickExpose = Animator.StringToHash("QuickExpose");
    private static readonly int Die = Animator.StringToHash("Die");

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
        AudioManager.PlaySound(AudioManager.Sound.CoreHit, transform.position);
        
        // Update the core states
        if (_currentHitPoints <= hitPoints * 0.75 && _currentHitPoints > hitPoints * 0.5)
        {
            _stateNumber = 1;
        }
        else if (_currentHitPoints <= hitPoints * 0.5 && _currentHitPoints > hitPoints * 0.25)
        {
            _stateNumber = 2;
        }
        else if (_currentHitPoints <= hitPoints * 0.25)
        {
            _stateNumber = 3;
        }
        
        GetComponent<Animator>().runtimeAnimatorController = coreStates[_stateNumber];
        GetComponent<Animator>().SetTrigger(QuickExpose);
        
        if (_currentHitPoints <= 0)
        {
            coreDestroyed = true;
            OnCoreDestroyed?.Invoke();
            CameraController.Instance.GetComponentInParent<CameraShake>().ShakeCamera(1f);
            GetComponent<Animator>().SetTrigger(Die);
            AudioManager.PlaySound(AudioManager.Sound.CoreDeath, transform.position);
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
