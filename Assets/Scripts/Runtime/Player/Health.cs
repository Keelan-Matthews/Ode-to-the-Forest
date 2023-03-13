using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    public static event Action OnPlayerDamaged;
    public static event Action OnPlayerHealed;
    public static event Action OnPlayerDeath;

    [SerializeField] private int health = 3;
    [SerializeField] private int maxHealth = 3;
    
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
            OnPlayerDeath?.Invoke();
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
