using UnityEngine;
using System;
using System.Collections;

public class Health : MonoBehaviour
{
    public static event Action OnPlayerDamaged;
    public static event Action OnPlayerHealed;
    public static event Action OnPlayerDeath;

    [SerializeField] private int health = 3;
    [SerializeField] private int maxHealth = 3;
    
    private SpriteRenderer _spriteRenderer;
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
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
        health -= damage;
        OnPlayerDamaged?.Invoke();

        if (health <= 0)
        {
            //See if tag is player
            if (gameObject.CompareTag("Player"))
            {
                OnPlayerDeath?.Invoke();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        // Invincibility frames
        if (gameObject.CompareTag("Player"))
        {
            StartCoroutine(InvincibilityFrames());
        }
    }
    
    private IEnumerator InvincibilityFrames()
    {
        // Make the sprite flash for the duration of the invincibility frames
        for (var i = 0; i < 3; i++)
        {
            _spriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.1f);
            _spriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
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
}
