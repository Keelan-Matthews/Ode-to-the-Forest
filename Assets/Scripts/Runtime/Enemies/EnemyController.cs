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
        transform.position = Vector2.MoveTowards(transform.position, targetPos, _speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            col.gameObject.GetComponent<Health>().TakeDamage(_damage);
            
            // Apply knockback to the player
            col.gameObject.GetComponent<KnockbackFeedback>().PlayFeedback(gameObject);
        }
    }
    
    public void SetDifficulty(int difficulty)
    {
        _speed += difficulty;
        _damage += difficulty;
    }

    public void Die()
    {
        // Drop essence using GameManager
        GameManager.Instance.DropEssence(_essenceToDrop, transform.position);

        // Destroy the enemy
        Destroy(gameObject);
    }
    
    
}
