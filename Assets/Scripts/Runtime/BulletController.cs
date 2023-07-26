using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public Rigidbody2D rb;
    private Animator _animator;
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
                // Stop the velocity of the bullet
                rb.velocity = Vector2.zero;
                var damage = PlayerController.Instance.FireDamage;
                DestroyObject();
                // Only apply damage if the enemy has health
                if (col.gameObject.GetComponent<Health>().HealthValue <= 0) return;
                col.gameObject.GetComponent<Health>().TakeDamage(damage);
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
