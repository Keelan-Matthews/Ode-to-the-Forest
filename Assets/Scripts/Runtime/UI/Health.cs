using UnityEngine;
using System;
using System.Collections;

public class Health : MonoBehaviour
{
    public static event Action OnPlayerDamaged;
    public static event Action OnPlayerHealed;
    public static event Action OnPlayerDeath;
    public static event Action OnAddedHeart;
    public static event Action OnRemovedHeart;

    [SerializeField] private int health;
    [SerializeField] private int maxHealth;
    
    private SpriteRenderer _spriteRenderer;
    private EnemyController _enemyController;
    public bool isInvincible;
    private bool _isDead;
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (gameObject.CompareTag("Enemy"))
        {
            _enemyController = GetComponent<EnemyController>();
        }
    }
    
    public int HealthValue
    {
        get => health;
        set => health = value;
    }
    
    public int MaxHealth
    {
        get => maxHealth;
        set => maxHealth = value;
    }
    
    public void TakeDamage(int damage, Transform attacker = null) 
    {
        if (isInvincible || _isDead) return;
        health -= damage;
        OnPlayerDamaged?.Invoke();
        
        // Get the sprite renderer of this object and change the color to hexadecimal
        _spriteRenderer.color = new Color(0.990566f, 0.4345407f, 0.4345407f);
        
        // Make the sprite color white again after 0.15 seconds
        StartCoroutine(ColorReset());

        if (health <= 0)
        {
            //See if tag is player
            if (gameObject.CompareTag("Player"))
            {
                _isDead = true;

                // Slow down time for 2 seconds, then speed it back up
                StartCoroutine(TimeSlow());
            }
            else if (gameObject.CompareTag("Enemy"))
            {
                _enemyController.Die();
                CameraController.Instance.GetComponentInParent<CameraShake>().ShakeCamera(0.1f, true);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            // Invincibility frames
            if (gameObject.CompareTag("Player"))
            {
                // Play the hit sound
                AudioManager.PlaySound(AudioManager.Sound.EnemyAttack, transform.position);
                // Apply knockback to the player
                gameObject.GetComponent<KnockbackFeedback>().PlayFeedback(attacker);
                StartCoroutine(InvincibilityFrames());

                if (health <= 2)
                {
                    PostProcessControls.Instance.SetLowHealthProfile();
                    PostProcessControls.Instance.isPulsing = true;
                    AudioManager.PlaySound(AudioManager.Sound.Heartbeat, transform.position);
                }
            }
        }
    }
    
    private IEnumerator TimeSlow()
    {
        Time.timeScale = 0.1f;
        RoomController.Instance.EnableDeathPostProcessing();
        yield return new WaitForSeconds(0.3f);
        Time.timeScale = 1f;
        RoomController.Instance.DisableDeathPostProcessing();
        AudioManager.PlaySound(AudioManager.Sound.OdeDeath, PlayerController.Instance.transform.position);
        OnPlayerDeath?.Invoke();
        PlayerController.Instance.GoToSleep();
    }
    
    public bool IsDead()
    {
        return _isDead;
    }
    
    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        // Make the sprite flash for the duration of the invincibility frames
        for (var i = 0; i < 2; i++)
        {
            _spriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.1f);
            _spriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
        isInvincible = false;
    }
    
    private IEnumerator ColorReset()
    {
        yield return new WaitForSeconds(0.15f);
        _spriteRenderer.color = Color.white;
    }

    public void Heal(int heal)
    {
        health += heal;
        OnPlayerHealed?.Invoke();
        
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        
        PostProcessControls.Instance.SetLowHealthProfile();
        PostProcessControls.Instance.isPulsing = false;
    }
    
    public void AddHeart()
    {
        OnAddedHeart?.Invoke();
    }

    public void RemoveHeart()
    {
        OnRemovedHeart?.Invoke();
    }
}
