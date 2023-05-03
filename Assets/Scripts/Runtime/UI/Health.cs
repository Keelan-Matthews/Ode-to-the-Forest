using UnityEngine;
using System;
using System.Collections;

public class Health : MonoBehaviour
{
    public static event Action OnPlayerDamaged;
    public static event Action OnPlayerHealed;
    public static event Action OnPlayerDeath;
    public static event Action OnAddedHeart;

    [SerializeField] private int health;
    [SerializeField] private int maxHealth;
    
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
        
        // Get the sprite renderer of this object and change the color to hexadecimal
        _spriteRenderer.color = new Color(0.990566f, 0.4345407f, 0.4345407f);
        
        // Make the sprite color white again after 0.15 seconds
        StartCoroutine(ColorReset());

        if (health <= 0)
        {
            //See if tag is player
            if (gameObject.CompareTag("Player"))
            {
                OnPlayerDeath?.Invoke();
            }
            else if (gameObject.CompareTag("Enemy"))
            {
                gameObject.GetComponent<EnemyController>().Die();
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
}
