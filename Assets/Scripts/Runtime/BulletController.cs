using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BulletController : MonoBehaviour
{
    public Rigidbody2D rb;
    private Animator _animator;
    public bool isSharpShooter;
    public bool isFreezePea;
    public bool isEnemyBullet;
    
    private float _ignoreTime = 0.01f;
    private float _ignoreTimer;
    
    [Header("Animator Runtime Controllers")]
    public RuntimeAnimatorController[] bulletAnimators;

    private static readonly int IsHit = Animator.StringToHash("IsHit");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        Invoke("Disable", 2f);
    }

    private void Update()
    {
        if (_ignoreTimer < _ignoreTime)
        {
            _ignoreTimer += Time.deltaTime;
        }
    }

    public void SetAnimatorPlayer()
    {
        isFreezePea = PlayerController.Instance.isFreezePea;
        isSharpShooter = PlayerController.Instance.isSharpShooter;
        
        if (isFreezePea)
        {
            _animator.runtimeAnimatorController = bulletAnimators[0];
        }
        
        // Update the bullet's animator if the player has the sharpshooter ability
        if (isSharpShooter)
        {
            _animator.runtimeAnimatorController = bulletAnimators[1];
        }
        
        if (isFreezePea && isSharpShooter)
        {
            // Change the sprite renderer color to blue
            GetComponent<SpriteRenderer>().color = new Color(0.1556604f, 0.8594025f, 1f, 1f);
        }
    }
    
    public void SetAnimatorElectric()
    {
        _animator.runtimeAnimatorController = bulletAnimators[2];
        
        // Get the light2D in the children and enable it
        var light2D = GetComponentInChildren<Light2D>();
        light2D.enabled = true;
    }
    
    public void SetAnimatorBoss()
    {
        _animator.runtimeAnimatorController = bulletAnimators[3];
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (_ignoreTimer < _ignoreTime && col.gameObject.CompareTag("Wall")) return;
        switch (col.gameObject.tag)
        {
            case "Enemy":
                if (isEnemyBullet) return;
                // Stop the velocity of the bullet
                rb.velocity = Vector2.zero;
                var bulletTransform1 = gameObject.transform;
                DestroyObject();
                var damage = PlayerController.Instance.FireDamage;
                // Only apply damage if the enemy has health
                if (col.gameObject.GetComponent<Health>().HealthValue <= 0) return;
                col.gameObject.GetComponent<Health>().TakeDamage(damage, bulletTransform1);
                // If freeze pea is enabled, slow the enemy
                if (isFreezePea)
                {
                    // Slow the enemy
                    col.gameObject.GetComponent<EnemyController>().SlowEnemy();
                }
                // Play the hit sound
                AudioManager.PlaySound(AudioManager.Sound.EnemyHit, transform.position);
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
                var bulletTransform = gameObject.transform;
                DestroyObject();
                // Only apply damage if the player has health
                if (col.gameObject.GetComponent<Health>().HealthValue <= 0) return;
                col.gameObject.GetComponent<Health>().TakeDamage(2, bulletTransform);
                break;
            case "Aim":
                if (isEnemyBullet) return;
                // Stop the velocity of the bullet
                rb.velocity = Vector2.zero;
                DestroyObject();
                if (BossController.Instance.isDead) return;
                var playerDamage = PlayerController.Instance.FireDamage;
                col.gameObject.GetComponentInParent<AimController>().TakeDamage(playerDamage);
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
