using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private EnemyData enemyData;
    private float _speed;
    private int _damage;
    private int _essenceToDrop;
    private NavMeshAgent _agent;
    private Animator _animator;
    private static readonly int SpawnRight = Animator.StringToHash("SpawnRight");
    private static readonly int SpawnLeft = Animator.StringToHash("SpawnLeft");

    private void Awake()
    {
        _speed = enemyData.speed;
        _damage = enemyData.damage;
        _essenceToDrop = enemyData.essenceToDrop;
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateUpAxis = false;
        _agent.updateRotation = false;
    }

    public void PlaySpawnAnimation()
    {
        // Get the player position
        var playerPosition = PlayerController.Instance.transform;
        
        // If the player is to the left of the enemy, set the SpawnRight trigger
        if (playerPosition.position.x < transform.position.x)
        {
            _animator.SetTrigger(SpawnLeft);
        }
        else
        {
            _animator.SetTrigger(SpawnRight);
        }
    }

    private void Update()
    {
        // Set the Z position to 0
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        
        // Get the movement vector
        var _movement = _agent.velocity;
        
        if (_movement.x != 0 || _movement.y != 0)
        {
            _animator.SetFloat("X", _movement.x);
            _animator.SetFloat("Y", _movement.y);

        }
    }

    public void MoveTowardsTarget(Vector2 targetPos)
    {
        _agent.SetDestination(targetPos);
        // transform.position = Vector2.MoveTowards(transform.position, targetPos, _speed * Time.deltaTime);
        
        // Move towards the player using context sensitive movement, avoiding obstacles
        // var direction = targetPos - (Vector2) transform.position;
        // var hit = Physics2D.Raycast(transform.position, direction, 10f, LayerMask.GetMask("Obstacle"));
        // if (hit.collider != null)
        // {
        //     // Move around the obstacle
        //     var angle = Vector2.SignedAngle(Vector2.right, direction);
        //     var newAngle = angle + 90;
        //     var newDirection = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad));
        //     transform.position = Vector2.MoveTowards(transform.position, targetPos + newDirection, _speed * Time.deltaTime);
        // }
        // else
        // {
        //     transform.position = Vector2.MoveTowards(transform.position, targetPos, _speed * Time.deltaTime);
        // }
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
