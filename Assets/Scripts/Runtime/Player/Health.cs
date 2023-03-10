using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private int _health;
    private int _maxHealth;
    
    public int HealthValue
    {
        get => _health;
        set => _health = value;
    }
    
    public int MaxHealth
    {
        get => _maxHealth;
        set => _maxHealth = value;
    }
    
    public void TakeDamage(int damage)
    {
        _health -= damage;
        if (_health <= 0)
        {
            //Die
        }
    }
    
    public void Heal(int heal)
    {
        _health += heal;
        if (_health > _maxHealth)
        {
            _health = _maxHealth;
        }
    }
}
