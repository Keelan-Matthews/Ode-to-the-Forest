using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed;
    [SerializeField] private int damage = 1;
    [SerializeField] private float rotateSpeed = 0.025f;
    [SerializeField] private int essenceToDrop = 3;

    public void MoveTowardsTarget(Vector2 targetPos)
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            col.gameObject.GetComponent<Health>().TakeDamage(damage);
            
            // Apply knockback to the player
            col.gameObject.GetComponent<KnockbackFeedback>().PlayFeedback(gameObject);
        }
    }
    
    public void SetDifficulty(int difficulty)
    {
        speed = difficulty * 2;
        damage = difficulty;
    }

    public void Die()
    {
        // Drop essence using GameManager
        GameManager.Instance.DropEssence(essenceToDrop, transform.position);

        // Destroy the enemy
        Destroy(gameObject);
    }
    
    
}
