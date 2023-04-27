using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    
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
                col.gameObject.GetComponent<Health>().TakeDamage(damage);
                // Apply knockback to the enemy
                col.gameObject.GetComponent<KnockbackFeedback>().PlayFeedback(gameObject);
                DestroyObject();
                break;
            case "Wall":
                // Stop the velocity of the bullet
                rb.velocity = Vector2.zero;
                DestroyObject();
                break;
            case "Obstacle":
                // Stop the velocity of the bullet
                rb.velocity = Vector2.zero;
                DestroyObject();
                break;
        }
    }

    private void DestroyObject()
    {
        _animator.SetBool(IsHit, true);
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
