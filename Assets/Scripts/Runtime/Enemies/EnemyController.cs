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
    private BehaviourController _behaviourController;
    private bool _isColliding;
    private bool _canAttack = true;
    private float _cooldownPeriod = 1f;
    private static readonly int SpawnRight = Animator.StringToHash("SpawnRight");
    private static readonly int SpawnLeft = Animator.StringToHash("SpawnLeft");
    private static readonly int X = Animator.StringToHash("X");
    private static readonly int Y = Animator.StringToHash("Y");
    private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
    private static readonly int Attack = Animator.StringToHash("Attack");
    
    private GameObject _player;
    private static readonly int DeathRight = Animator.StringToHash("DeathRight");
    private static readonly int DeathLeft = Animator.StringToHash("DeathLeft");

    private void Awake()
    {
        _speed = enemyData.speed;
        _damage = enemyData.damage;
        _essenceToDrop = enemyData.essenceToDrop;
        _animator = GetComponent<Animator>();
        _behaviourController = GetComponent<BehaviourController>();
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
        if (_animator == null) return;
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
        
        if (_animator == null) return;
        // Get the movement vector
        var _movement = _agent.velocity;
        
        if (_movement.x != 0 || _movement.y != 0)
        {
            _animator.SetFloat(X, _movement.x);
            _animator.SetFloat(Y, _movement.y);
        }

        if (_isColliding)
        {
            HandleAttack();
        }
    }

    public void MoveTowardsTarget(Vector2 targetPos)
    {
        _agent.SetDestination(targetPos);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        // Only damage the player, and only do so once the enemy has spawned
        if (!col.gameObject.CompareTag("Player") || !GetComponent<BehaviourController>().IsSpawned()) return;
        _player = col.gameObject;
        _isColliding = true;
    }

    private void HandleAttack()
    {
        if (!_canAttack) return;
        
        _player.GetComponent<Health>().TakeDamage(_damage);

        // Play the enemy hit sound for now
        AudioManager.PlaySound(AudioManager.Sound.EnemyHit, transform.position);
            
        // Apply knockback to the player
        // if (_player.GetComponent<Health>().isInvincible) return;
        _player.GetComponent<KnockbackFeedback>().PlayFeedback(gameObject);
        _animator.SetTrigger(Attack);
        
        // Start the cooldown
        StartCoroutine(Cooldown());
    }
    
    private IEnumerator Cooldown()
    {
        _canAttack = false;
        yield return new WaitForSeconds(_cooldownPeriod);
        _canAttack = true;
    }
    
    private void OnCollisionExit2D(Collision2D col)
    {
        // if it is not the player, return
        if (!col.gameObject.CompareTag("Player")) return;
        _isColliding = false;
    }

    public void SetDifficulty(int difficulty)
    {
        _speed += difficulty/4;
        _damage += difficulty/4;
    }
    
    public void StopMoving()
    {
        _agent.isStopped = true;
    }

    public void Die()
    {
        // Change the behavior to stop the enemy from moving
        _behaviourController.SetAI("EnemyFreeze");
        
        // Play the death animation left or right depending on the player's position
        var playerPosition = PlayerController.Instance.transform;
        if (playerPosition.position.x < transform.position.x)
        {
            _animator.SetTrigger(DeathLeft);
        }
        else
        {
            _animator.SetTrigger(DeathRight);
        }
        // Drop essence using GameManager
        GameManager.Instance.DropEssence(_essenceToDrop, transform.position);
        // Drop a perma seed
        GameManager.Instance.DropPermaSeed(transform.position);

        // Destroy the enemy after a delay
        StartCoroutine(DestroyAfterDelay());
    }
    
    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        // If the enemy is still alive, destroy it
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}
