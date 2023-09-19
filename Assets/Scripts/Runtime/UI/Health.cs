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
    
    public void TakeDamage(int damage)
    {
        if (isInvincible || _isDead) return;
        health -= damage;
        OnPlayerDamaged?.Invoke();

        if (health <= 0)
        {
            //See if tag is player
            if (gameObject.CompareTag("Player"))
            {
                OnPlayerDeath?.Invoke();
                _isDead = true;
                AudioManager.PlaySound(AudioManager.Sound.OdeDeath, PlayerController.Instance.transform.position);
            }
            else if (gameObject.CompareTag("Enemy"))
            {
                _enemyController.Die();
                CameraController.Instance.GetComponentInParent<CameraShake>().ShakeCamera(0.1f);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            // Get the sprite renderer of this object and change the color to hexadecimal
            _spriteRenderer.color = new Color(0.990566f, 0.4345407f, 0.4345407f);
        
            // Make the sprite color white again after 0.15 seconds
            StartCoroutine(ColorReset());
            
            // Invincibility frames
            if (gameObject.CompareTag("Player"))
            {
                // Play the hit sound
                AudioManager.PlaySound(AudioManager.Sound.EnemyAttack, transform.position);
                // Apply knockback to the player
                gameObject.GetComponent<KnockbackFeedback>().PlayFeedback(gameObject);
                StartCoroutine(InvincibilityFrames());
            }
        }
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
