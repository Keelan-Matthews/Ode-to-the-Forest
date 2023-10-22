using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CoreController : MonoBehaviour
{
    [SerializeField] private int hitPoints;
    private int _currentHitPoints;
    public bool coreDestroyed;
    public bool canTakeDamage;
    [SerializeField] private RuntimeAnimatorController[] coreStates;
    [SerializeField] private Light2D coreLight;
    private int _stateNumber;
    private static readonly int QuickExpose = Animator.StringToHash("QuickExpose");
    private static readonly int Die = Animator.StringToHash("Die");
    private static readonly int Protect = Animator.StringToHash("Protect");
    private static readonly int Expose = Animator.StringToHash("Expose");

    public static event Action OnCoreDestroyed;
    public static event Action<int> OnCoreHit;
    
    private float _coreLightIntensity;
    
    private void Awake()
    {
        _currentHitPoints = hitPoints;
        _coreLightIntensity = coreLight.intensity;
        coreLight.intensity = 0;
    }
    
    public void TakeDamage(int damage)
    {
        if (_currentHitPoints <= 0) return;
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
        if (canTakeDamage)
        {
            GetComponent<Animator>().SetTrigger(QuickExpose);
        }

        if (_currentHitPoints <= 0)
        {
            coreDestroyed = true;
            OnCoreDestroyed?.Invoke();
            CameraController.Instance.GetComponentInParent<CameraShake>().ShakeCamera(1f);
            GetComponent<Animator>().SetTrigger(Die);
            AudioManager.PlaySound(AudioManager.Sound.CoreDeath, transform.position);
            StartCoroutine(DimCore(true));
        }
        
        StartCoroutine(FlashRed());
    }
    
    private IEnumerator FlashRed()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
        
        if (!canTakeDamage)
        {
            GetComponent<Animator>().SetTrigger(Protect);
        }
    }
    
    private IEnumerator BrightenCore()
    {
        var duration = 0.3f;
        while (coreLight.intensity < _coreLightIntensity)
        {
            // If the core is destroyed, stop the coroutine
            if (coreDestroyed)
            {
                StartCoroutine(DimCore());
                yield break;
            }
            coreLight.intensity += Time.deltaTime / duration;
            yield return null;
        }
        
        if (coreDestroyed) yield break;
        
        // Flash the core 3 times
        for (var i = 0; i < 3; i++)
        {
            coreLight.intensity = 0;
            yield return new WaitForSeconds(0.1f);
            coreLight.intensity = _coreLightIntensity;
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    private IEnumerator DimCore(bool destroyLight = false)
    {
        var duration = 0.3f;
        while (coreLight.intensity > 0)
        {
            coreLight.intensity -= Time.deltaTime / duration;
            yield return null;
        }
        
        if (destroyLight)
        {
            Destroy(coreLight.gameObject);
        }
    }
    
    public void ExposeCore()
    {
        StartCoroutine(BrightenCore());
        GetComponent<Animator>().SetTrigger(Expose);
    }
    
    public void ProtectCore()
    {
        StartCoroutine(DimCore());
        GetComponent<Animator>().SetTrigger(Protect);
        canTakeDamage = false;
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
