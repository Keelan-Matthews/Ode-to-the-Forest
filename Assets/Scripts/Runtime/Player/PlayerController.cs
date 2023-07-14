using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour, IDataPersistence
{
    #region Variables
    [SerializeField] private int speed = 5;
    [SerializeField] private float fireForce = 7f;
    [SerializeField] private float cooldownPeriod = 0.8f;
    [SerializeField] private int fireDamage = 4;
    private float _bulletRange = 0.3f;
    private List<AbilityEffect> _abilities; // The abilities the player has equipped
    // Stores a single perma seed picked up in the dungeon
    private PermaSeed _permaSeed;
    public int essenceFragments; // The currency of the game
    public int essence;
    private const int EssenceQuantity = 5;
    #endregion
    #region References
    public static PlayerController Instance;
    public EssenceMeter essenceMeter;
    [SerializeField] private Camera sceneCamera;
    #endregion
    #region Movement Properties
    private Vector2 _movement;
    private Vector3 _mouseWorldPosition;
    private Rigidbody2D _rb;
    private Animator _animator;
    private Vector2 _smoothedMovement;
    private Vector2 _movementInputSmoothVelocity;
    #endregion
    #region Shooting Properties
    private bool _canShoot = true;
    private bool _isShooting;
    private bool _isMoving;
    public bool inSunlight = true;
    public bool inCloud = false;
    private bool _isAiming = false;
    #endregion
    private Health _health;
    private bool _playerExists;
    #region Animation Hashes
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int Y = Animator.StringToHash("Y");
    private static readonly int X = Animator.StringToHash("X");
    private static readonly int Upgrade = Animator.StringToHash("Upgrade");
    private static readonly int Downgrade = Animator.StringToHash("Downgrade");
    #endregion
    private void Awake()
    {
        Instance = this;
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();
        _abilities = new List<AbilityEffect>();
    }

    #region Movement and Shooting
    public void OnMovement(InputAction.CallbackContext context)
    {
        // If there is active dialogue or the player is dead, don't allow movement
        if (GameManager.Instance.activeDialogue || _health.HealthValue == 0)
        {
            // set the movement to 0 so the player doesn't move
            _movement = Vector2.zero;
            _animator.SetBool(IsWalking, false);

            // Make Ode face forward
            _animator.SetFloat(X, 0);
            _animator.SetFloat(Y, -1);
            return;
        }

        // Get the value from the input system
        _movement = context.ReadValue<Vector2>();
        _isMoving = context.control.IsPressed();

        // Update the animator with the new movement values so it can play the correct animation
        if (_movement.x != 0 || _movement.y != 0)
        {
            if (!_isShooting)
            {
                _animator.SetFloat(X, _movement.x);
                _animator.SetFloat(Y, _movement.y);
            }

            _animator.SetBool(IsWalking, true); //Tell the animator that the player is moving
        }
        else
        {
            _animator.SetBool(IsWalking, false); //Tell the animator that the player is not moving
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        _isShooting = context.control.IsPressed();
    }

    private void HandleShoot()
    {
        // Check if the player can shoot and if they are in the sunlight
        if (!_canShoot || !inSunlight || inCloud) return;
        if (GameManager.Instance.activeDialogue || _health.HealthValue == 0) return;

        // Play the shoot sound
        AudioManager.PlaySound(AudioManager.Sound.PlayerShoot, transform.position);

        // Check if the player has the scattershot ability
        if (IsScattershot)
        {
            Scattershot();
        }
        else
        {
            Shoot();
        }

        // Start the cooldown
        StartCoroutine(Cooldown());
    }

    private void Shoot(int i = 0)
    {
        // Get a bullet instance from the pool and set its position to the player's position
        var obj = ObjectPooler.Instance.GetPooledObject();
        if (obj == null) return;

        var t = transform;
        obj.transform.position = t.position;
        obj.transform.rotation = t.rotation;
        obj.SetActive(true);

        // Shoot the object in the direction of the mouse
        var direction = _mouseWorldPosition - transform.position;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle += i switch
        {
            0 => 0,
            1 => -10,
            _ => 10
        };
        var newDirection = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right;
        obj.GetComponent<Rigidbody2D>().velocity = new Vector2(newDirection.x, newDirection.y).normalized * fireForce;

        // After the bullet has traveled a certain distance, destroy it
        StartCoroutine(DestroyBullet(obj));
    }

    private void Scattershot()
    {
        // Shoot a regular shoot, but also fire one bullet angled to the left of that bullet,
        // and another to the right of it. Therefore shooting 3 bullets at once

        for (var i = 0; i < 3; i++)
        {
            Shoot(i);
        }
    }

    private IEnumerator DestroyBullet(GameObject obj)
    {
        yield return new WaitForSeconds(_bulletRange);

        // Get the bullet controller script and call destroy on it
        if (obj == null) yield break;
        var bulletController = obj.GetComponent<BulletController>();

        // If the bullet controller is null, then the bullet is not a bullet, so just destroy it
        if (bulletController == null) yield break;
        bulletController.DestroyObject();
    }

    private IEnumerator Cooldown()
    {
        _canShoot = false;
        yield return new WaitForSeconds(cooldownPeriod);
        _canShoot = true;
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.activeDialogue || _health.HealthValue == 0 || PauseMenu.GameIsPaused) return;
        _isAiming = context.control.IsPressed();

        // Get the value from the input system and convert it to a Vector2
        var aimPosition = context.ReadValue<Vector2>();

        // If input method is mouse, then the aim position is the mouse position
        if (context.control.device is Mouse)
        {
            _mouseWorldPosition = sceneCamera.ScreenToWorldPoint(aimPosition);
        }
        else
        {
            // If input method is controller, then the aim position is the player's position + the aim position
            _mouseWorldPosition = transform.position + new Vector3(aimPosition.x, aimPosition.y, 0);
        }

        var direction = _mouseWorldPosition - transform.position;

        // Update the animator with the new movement values so it can play the correct animation
        _animator.SetFloat(X, direction.x);
        _animator.SetFloat(Y, direction.y);
    }
    
    private void FixedUpdate()
    {
        if (GameManager.Instance.activeDialogue || _health.HealthValue == 0)
        {
            // Set the velocity to 0 so the player doesn't move
            _rb.velocity = Vector2.zero;
            _movement = Vector2.zero;

            // Make sure the player is not moving
            _animator.SetBool(IsWalking, false);
            // Make Ode face forward
            _animator.SetFloat(X, 0);
            _animator.SetFloat(Y, -1);
            return;
        }

        // Smooth the movement input
        _smoothedMovement = Vector2.SmoothDamp(_smoothedMovement, _movement, ref _movementInputSmoothVelocity, 0.1f);
        _rb.velocity = _smoothedMovement * speed;

        // Handle shooting
        if (_isShooting)
        {
            HandleShoot();
        }

        if (_isMoving)
        {
            // Play the movement sound
            AudioManager.PlaySound(AudioManager.Sound.PlayerWalk, transform.position);
        }

        // See if the player is within the sunlight circle or box collider
        var tp = transform.position;
        inSunlight = Physics2D.OverlapCircle(tp, 0.5f, LayerMask.GetMask("Sunlight"))
                     || Physics2D.OverlapBox(tp, new Vector2(0.5f, 0.5f), 0, LayerMask.GetMask("Sunlight"));
    }
    #endregion
    

    public void TakeDamage(int damage)
    {
        _health.TakeDamage(damage);
    }

    public void PlayUpgradeAnimation()
    {
        _animator.SetTrigger(Upgrade);
    }

    public void PlayDowngradeAnimation()
    {
        _animator.SetTrigger(Downgrade);
    }

    public bool IsInvincible()
    {
        return _health.isInvincible;
    }

    public void SetInvincible(bool invincible)
    {
        _health.isInvincible = invincible;
    }
    
    #region Getters and Setters
    public int Speed
    {
        get => speed;
        set => speed = value;
    }

    public float CooldownPeriod
    {
        get => cooldownPeriod;
        set => cooldownPeriod = value;
    }

    public float FireForce
    {
        get => fireForce;
        set => fireForce = value;
    }

    public bool IsScattershot { get; set; }

    public int FireDamage
    {
        get => fireDamage;
        set => fireDamage = value;
    }

    public float BulletRange
    {
        get => _bulletRange;
        set => _bulletRange = value;
    }
    #endregion

    #region Essence and Abilities

    public void AddEssence(int amount)
    {
        // Add essence to the player's essence fragments
        essenceFragments += amount;

        // Update the essence meter
        essenceMeter.SetEssenceFragment(essenceFragments);

        // Essence = 5 essence fragments
        if (essenceFragments < EssenceQuantity) return;

        // If the player has enough essence fragments, then add an essence
        essenceFragments -= EssenceQuantity;
        essence++;

        // Reset the essence meter
        essenceMeter.SetEssenceFragment(0);

        // Update the UI
        GameManager.Instance.UpdateEssenceUI(essence);
    }

    public void AddAbility(AbilityEffect ability)
    {
        // Add an ability to the player's abilities
        _abilities.Add(ability);

        // Apply the ability
        ability.Apply(gameObject);
    }

    public void RemoveAbility(AbilityEffect ability)
    {
        // Remove an ability from the player's abilities
        _abilities.Remove(ability);
    }

    public bool HasAbility(AbilityEffect ability)
    {
        // Check if the player has an ability
        return _abilities.Contains(ability);
    }

    public int GetEssence()
    {
        // Get the player's essence
        return essence;
    }

    public void ResetEssence()
    {
        // Reset the player's essence
        essence = 0;

        // Also reset players fragments
        essenceFragments = 0;
    }

    public void ResetAbilities()
    {
        // Reset the player's abilities
        _abilities.Clear();
    }

    public int GetEssenceFragments()
    {
        // Get the player's essence fragments
        return essenceFragments;
    }

    public void SpendEssence(int amount)
    {
        essence -= amount;
        // Update the UI
        GameManager.Instance.UpdateEssenceUI(essence);
    }

    public bool AddPermaSeed(PermaSeed seed)
    {
        // If the player already has a perma seed in their inventory, return false
        if (_permaSeed != null) return false;

        // Add the perma seed to the player's inventory
        _permaSeed = seed;
        // Update the inventory UI
        InventoryManager.Instance.AddPermaSeed(seed);

        return true;
    }

    public bool HasSeed()
    {
        // Check if the player has a perma seed
        return _permaSeed != null;
    }

    public bool HasSeed(PermaSeed seed)
    {
        // Check if the player has a specific perma seed
        return _permaSeed == seed;
    }

    public PermaSeed PlantSeed()
    {
        // Get the player's perma seed
        var seed = _permaSeed;
        // Remove the perma seed from the player's inventory
        _permaSeed = null;
        // Update the inventory UI
        InventoryManager.Instance.RemovePermaSeed();

        return seed;
    }

    #endregion
    
    public void LoadData(GameData data)
    {
        //Apply any abilities the player has
        foreach (var ability in data.Abilities)
        {
            Instance.AddAbility(ability);
        }
        
        // Add the perma seed to the player's inventory
        _permaSeed = data.PermaSeed;
        // Update the inventory UI
        InventoryManager.Instance.AddPermaSeed(_permaSeed);
        
        // Load the essence
        essence = data.Essence;
        // Load the essence fragments
        essenceFragments = data.EssenceFragments;
    }

    public void SaveData(GameData data)
    {
        // Save the player's position
        data.PlayerPosition = transform.position;
        // Save the player's abilities
        data.Abilities = _abilities;
        // Save the player's perma seed
        data.PermaSeed = _permaSeed;
        // Save the player's essence
        data.Essence = essence;
        // Save the player's essence fragments
        data.EssenceFragments = essenceFragments;
    }
}