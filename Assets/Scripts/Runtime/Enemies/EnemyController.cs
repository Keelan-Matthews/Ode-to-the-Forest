using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    private float _speed;
    private int _damage;
    private int _essenceToDrop;
    
    private void Awake()
    {
        _speed = enemyData.speed;
        _damage = enemyData.damage;
        _essenceToDrop = enemyData.essenceToDrop;
    }

    public void MoveTowardsTarget(Vector2 targetPos)
    {
        // transform.position = Vector2.MoveTowards(transform.position, targetPos, _speed * Time.deltaTime);
        
        // Move towards the player using context sensitive movement, avoiding obstacles
        var direction = targetPos - (Vector2) transform.position;
        var hit = Physics2D.Raycast(transform.position, direction, 10f, LayerMask.GetMask("Obstacle"));
        if (hit.collider != null)
        {
            // Move around the obstacle
            var angle = Vector2.SignedAngle(Vector2.right, direction);
            var newAngle = angle + 90;
            var newDirection = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad));
            transform.position = Vector2.MoveTowards(transform.position, targetPos + newDirection, _speed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, _speed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;
        col.gameObject.GetComponent<Health>().TakeDamage(_damage);
        
        // Play the enemy hit sound for now
        AudioManager.PlaySound(AudioManager.Sound.EnemyHit, transform.position);
            
        // Apply knockback to the player
        // if (col.gameObject.GetComponent<Health>().isInvincible) return;
        col.gameObject.GetComponent<KnockbackFeedback>().PlayFeedback(gameObject);
    }
    
    public void SetDifficulty(int difficulty)
    {
        _speed += difficulty/4;
        _damage += difficulty/4;
    }

    public void Die()
    {
        // Drop essence using GameManager
        GameManager.Instance.DropEssence(_essenceToDrop, transform.position);

        // Destroy the enemy
        Destroy(gameObject);
    }
    
    
}
