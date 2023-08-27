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
    private bool _canShoot = false;
    private const float CooldownPeriod = 0.3f;
    private Health _playerHealth;

    [Header("Projectile enemy")] 
    [SerializeField] private bool isProjectileEnemy;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float fireForce = 7f;
    private float shootCooldownPeriod = 2f;
    private Health _health;

    #region Animation Hashes

    private static readonly int SpawnRight = Animator.StringToHash("SpawnRight");
    private static readonly int SpawnLeft = Animator.StringToHash("SpawnLeft");
    private static readonly int X = Animator.StringToHash("X");
    private static readonly int Y = Animator.StringToHash("Y");
    private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int DeathRight = Animator.StringToHash("DeathRight");
    private static readonly int DeathLeft = Animator.StringToHash("DeathLeft");

    #endregion

    private GameObject _player;

    private void Awake()
    {
        _speed = enemyData.speed;
        _damage = enemyData.damage;
        _essenceToDrop = enemyData.essenceToDrop;
        _animator = GetComponent<Animator>();
        _behaviourController = GetComponent<BehaviourController>();
        _health = GetComponent<Health>();
        _playerHealth = PlayerController.Instance.GetComponent<Health>();

        // Subscribe to on player death
        Health.OnPlayerDeath += Health_OnPlayerDeath;
    }

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateUpAxis = false;
        _agent.updateRotation = false;
        
        // Set the agent speed
        _agent.speed = _speed;
        
        StartCoroutine(ShootCooldown());
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

        // Play spawn audio
        AudioManager.PlaySound(AudioManager.Sound.EnemySpawn, transform.position);
    }

    private void Update()
    {
        // Set the Z position to 0
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        
        // IF the player is alive, get the player's position, else get the movement vector
        var _movement = _playerHealth.HealthValue > 0 ? _player.transform.position : _agent.velocity;

        if (_movement.x != 0 || _movement.y != 0)
        {
            if (_animator != null)
            {
                _animator.SetFloat(X, _movement.x);
                _animator.SetFloat(Y, _movement.y);
            }
        }

        if (_isColliding && _health.HealthValue > 0)
        {
            HandleAttack();
        } 
        
        if (isProjectileEnemy && _canShoot && _health.HealthValue > 0)
        {
            HandleShoot();
        }
    }

    public void MoveTowardsTarget(Vector2 targetPos)
    {
        _agent.SetDestination(targetPos);
    }
    
    private void HandleShoot()
    {
        // Check if the player can shoot and if they are in the sunlight
        if (!_canShoot) return;
        // If the player is dead, don't shoot
        if (PlayerController.Instance.GetComponent<Health>().HealthValue <= 0) return;

        // Play the shoot sound
        AudioManager.PlaySound(AudioManager.Sound.PlayerShoot, transform.position);
        
        Shoot();

        // Start the cooldown
        StartCoroutine(ShootCooldown());
    }
    
    private void Shoot()
    {
        // Get a bullet instance from the pool and set its position to the player's position
        var obj = ObjectPooler.Instance.GetPooledObject();
        if (obj == null) return;
        
        obj.GetComponent<BulletController>().isEnemyBullet = true;
        obj.GetComponent<BulletController>().SetAnimatorElectric();

        var t = transform;
        obj.transform.position = t.position;
        obj.transform.rotation = t.rotation;
        obj.SetActive(true);

        // Shoot the object towards the player
        obj.GetComponent<Rigidbody2D>().velocity = (PlayerController.Instance.transform.position - transform.position).normalized * fireForce;
    }

    // Function to check if the enemy has reached its current destination
// You may need to adjust the threshold value to fit your game's needs
    public bool HasReachedDestination()
    {
        // Define a tolerance distance to consider the enemy as having reached the destination
        var tolerance = 5f; // Adjust this value as needed

        // Calculate the distance between the enemy's current position and the target destination
        var distanceToTarget = Vector2.Distance(transform.position, _agent.destination);
        
        // Check if the enemy is within the tolerance distance from the target destination
        return distanceToTarget <= tolerance;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        // Only damage the player, and only do so once the enemy has spawned
        if (!col.gameObject.CompareTag("Player") || !GetComponent<BehaviourController>().IsSpawned()) return;
        _player = col.gameObject;
        _isColliding = true;
    }

    // When the player dies ,set the behavior to wander
    private void Health_OnPlayerDeath()
    {
        _behaviourController.SetAI("EnemyWander");
    }

    private void HandleAttack()
    {
        if (!_canAttack || _health.HealthValue <= 0) return;

        // Play the enemy attack sound
        AudioManager.PlaySound(AudioManager.Sound.EnemyAttack, transform.position);

        // Apply knockback to the player
        // if (_player.GetComponent<Health>().isInvincible) return;
        if (_animator != null)
        {
            _animator.SetTrigger(Attack);
        }

        StartCoroutine(DamagePlayer());

        // Start the cooldown
        StartCoroutine(Cooldown());
    }
    
    private IEnumerator DamagePlayer()
    {
        // 0.5 second delay
        yield return new WaitForSeconds(0.2f);
        // check if still colliding
        if (!_isColliding) yield break;
        _player.GetComponent<Health>().TakeDamage(_damage);
        _player.GetComponent<KnockbackFeedback>().PlayFeedback(gameObject);
    }

    private IEnumerator Cooldown()
    {
        _canAttack = false;
        yield return new WaitForSeconds(CooldownPeriod);
        _canAttack = true;
    }
    
    private IEnumerator ShootCooldown()
    {
        _canShoot = false;
        // If the player has freezepea, double the cooldown
        if (PlayerController.Instance.isFreezePea)
        {
            shootCooldownPeriod = 3f;
        }
        else
        {
            shootCooldownPeriod = 2f;
        }
        yield return new WaitForSeconds(shootCooldownPeriod);
        _canShoot = true;
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        // if it is not the player, return
        if (!col.gameObject.CompareTag("Player")) return;
        _isColliding = false;
    }

    public void SetDifficulty(int difficulty)
    {
        _speed += difficulty / 4;
        _damage += difficulty / 4;
    }

    public void StopMoving()
    {
        _agent.isStopped = true;
    }

    public void SlowEnemy()
    {
        // Stop the enemy for 2 seconds
        _agent.speed = 0.2f;
        
        // Slow down the animation speed if they still have health
        if (_health.HealthValue > 0 && _animator != null)
        {
            _animator.speed = 0.2f;
        }

        // Get the sprite renderer and change the color to blue
        var spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(0.1556604f, 0.8594025f, 1f, 1f);
        
        StartCoroutine(ResetSpeed());
    }
    
    private IEnumerator ResetSpeed()
    {
        yield return new WaitForSeconds(2f);
        _agent.speed = _speed;
        
        // Reset the animation speed
        _animator.speed = 1f;
        
        // Reset the color
        var spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.white;
    }

    public void Die()
    {
        // Change the behavior to stop the enemy from moving
        _behaviourController.SetAI("EnemyFreeze");

        // Play the death sound
        AudioManager.PlaySound(AudioManager.Sound.EnemyDeath, transform.position);

        // Play the death animation left or right depending on the player's position
        var playerPosition = PlayerController.Instance.transform;
        if (_animator)
        {
            if (playerPosition.position.x < transform.position.x)
            {
                _animator.SetTrigger(DeathLeft);
            }
            else
            {
                _animator.SetTrigger(DeathRight);
            }
        }

        // Drop essence using GameManager
        GameManager.Instance.DropEssence(_essenceToDrop, transform.position);

        // Destroy the enemy after a delay
        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(0.6f);
        // If the enemy is still alive, destroy it
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}