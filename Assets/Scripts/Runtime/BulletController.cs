using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public Rigidbody2D rb;
    private Animator _animator;
    public bool isSharpShooter;
    public bool isFreezePea;
    public bool isEnemyBullet;

    private static readonly int IsHit = Animator.StringToHash("IsHit");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        Invoke("Disable", 2f);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        switch (col.gameObject.tag)
        {
            case "Enemy":
                if (isEnemyBullet) return;
                // Stop the velocity of the bullet
                rb.velocity = Vector2.zero;
                var damage = PlayerController.Instance.FireDamage;
                DestroyObject();
                // Only apply damage if the enemy has health
                if (col.gameObject.GetComponent<Health>().HealthValue <= 0) return;
                col.gameObject.GetComponent<Health>().TakeDamage(damage);
                
                // If freeze pea is enabled, slow the enemy
                if (isFreezePea)
                {
                    // Slow the enemy
                    col.gameObject.GetComponent<EnemyController>().SlowEnemy();
                }

                // Play the hit sound
                AudioManager.PlaySound(AudioManager.Sound.EnemyHit, transform.position);
                // Apply knockback to the enemy
                col.gameObject.GetComponent<KnockbackFeedback>().PlayFeedback(gameObject);
                break;
            case "Wall":
                DestroyObject();
                break;
            case "Obstacle":
                DestroyObject();
                // If sharp shooter is enabled, break the obstacle
                if (isSharpShooter)
                {
                    // Destroy the obstacle
                    Destroy(col.gameObject);
                }
                break;
            case "Player":
                if (!isEnemyBullet) return;
                // Stop the velocity of the bullet
                rb.velocity = Vector2.zero;
                DestroyObject();
                // Only apply damage if the player has health
                if (col.gameObject.GetComponent<Health>().HealthValue <= 0) return;
                col.gameObject.GetComponent<Health>().TakeDamage(2);
                // Play the hit sound
                AudioManager.PlaySound(AudioManager.Sound.PlayerHit, transform.position);
                // Apply knockback to the player
                col.gameObject.GetComponent<KnockbackFeedback>().PlayFeedback(gameObject);
                break;
        }
    }

    public void DestroyObject()
    {
        rb.velocity = Vector2.zero;
        _animator.SetBool(IsHit, true);
        // Set the bullet to inactive after the animation has played
        // Invoke("Disable", 0.2f);
        if (gameObject == null) return;
        Destroy(gameObject, 0.2f);
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        CancelInvoke();
    }
}
