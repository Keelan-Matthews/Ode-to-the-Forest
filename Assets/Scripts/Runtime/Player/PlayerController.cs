using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Runtime.Abilities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Cursor = UnityEngine.UIElements.Cursor;

public class PlayerController : MonoBehaviour, IDataPersistence
{
    #region Variables
    [SerializeField] private int speed = 5;
    [SerializeField] private float fireForce = 7f;
    [SerializeField] private float cooldownPeriod = 0.8f;
    [SerializeField] private int fireDamage = 4;
    private float _bulletRange = 0.3f;
    [SerializeField] private List<AbilityEffect> abilities; // The abilities the player has equipped
    public bool isSleeping;
    
    public int essenceFragments; // The currency of the game
    public int essence;
    private const int EssenceQuantity = 5;
    #endregion
    #region References
    public static PlayerController Instance;
    public EssenceMeter essenceMeter;
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private CircleCollider2D essenceCollector;
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
    private bool _hasWokenUp;
    public bool isFreezePea;
    public bool isSharpShooter;
    public bool inSunlight = true;
    public bool isCorrupted = false;
    public bool inCloud = false;
    #endregion
    private Health _health;
    private bool _playerExists;
    
    [Header("Particle Emitters")]
    [SerializeField] private ParticleSystem upgradeParticles;
    [SerializeField] private ParticleSystem downgradeParticles;
    
    #region Animation Hashes
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int Y = Animator.StringToHash("Y");
    private static readonly int X = Animator.StringToHash("X");
    private static readonly int Upgrade = Animator.StringToHash("Upgrade");
    private static readonly int Downgrade = Animator.StringToHash("Downgrade");
    private static readonly int Sleep = Animator.StringToHash("Sleep");
    private static readonly int Up = Animator.StringToHash("WakeUp");

    #endregion
    private void Awake()
    {
        Instance = this;
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();
        abilities = new List<AbilityEffect>();
    }

    #region Movement and Shooting
    public void OnMovement(InputAction.CallbackContext context)
    {
        // If the player is sleeping, wake them up
        if (isSleeping)
        {
            WakeUp();
        }
        
        // If there is active dialogue or the player is dead, don't allow movement
        if (GameManager.Instance.activeDialogue || _health.HealthValue <= 0)
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
        
        // Accomodate for the player moving diagonally by ensuring that _isMoving is true if the player is moving
        _isMoving = _movement.x != 0 || _movement.y != 0;

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
        
        // If the active scene is the home scene or the player is dead
        if (ScenesManager.Instance.currentSceneName == "Home")
        {
            GameManager.Instance.SetCursorDefault();
        }
        else
        {
            // If Ode is un the sunlight, set the cursor to shoot cursor
            if (inSunlight && !inCloud)
            {
                GameManager.Instance.SetCursorShoot();
            }
            else
            {
                GameManager.Instance.SetCursorCannotShoot();
            }
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (isSleeping || GameManager.Instance.activeDialogue || _health.HealthValue <= 0) return;
        _isShooting = context.control.IsPressed();
    }

    private void HandleShoot()
    {
        // Check if the player can shoot and if they are in the sunlight
        if (!_canShoot || !inSunlight || inCloud) return;
        if (GameManager.Instance.activeDialogue || _health.HealthValue <= 0) return;

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
    
    public bool ConvertIfCorrupted(bool sunlight)
    {
        // If the player is corrupted, invert the value of sunlight
        if (isCorrupted)
        {
            return !sunlight;
        }
        
        return sunlight;
    }

    private void Shoot(int i = 0)
    {
        // Get a bullet instance from the pool and set its position to the player's position
        var obj = ObjectPooler.Instance.GetPooledObject();
        if (obj == null) return;
        
        obj.GetComponent<BulletController>().isFreezePea = isFreezePea;
        obj.GetComponent<BulletController>().isSharpShooter = isSharpShooter;

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
    
    public void EnableEssenceMagnet()
    {
        // Increase the radius of the essence collector by 2
        essenceCollector.radius = 6;
    }
    
    public void DisableEssenceMagnet()
    {
        // Decrease the radius of the essence collector by 2
        essenceCollector.radius = 1.8f;
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
        if (GameManager.Instance.activeDialogue || _health.HealthValue <= 0 || PauseMenu.GameIsPaused || isSleeping) return;
        // _isAiming = context.control.IsPressed();

        // Get the value from the input system and convert it to a Vector2
        var aimPosition = context.ReadValue<Vector2>();
        
        if (sceneCamera == null) return;

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
        
        // If the active scene is the home scene or the player is dead
        if (ScenesManager.Instance.currentSceneName == "Home" || _health.HealthValue <= 0)
        {
            GameManager.Instance.SetCursorDefault();
        }
        else
        {
            // If Ode is un the sunlight, set the cursor to shoot cursor
            if (inSunlight && !inCloud)
            {
                GameManager.Instance.SetCursorShoot();
            }
            else
            {
                GameManager.Instance.SetCursorCannotShoot();
            }
        }
    }
    
    private void FixedUpdate()
    {
        if (_health.HealthValue <= 0 || (GameManager.Instance != null && GameManager.Instance.activeDialogue))
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
        
        // Make sure left click is pressed when is shooting is true, else make it false
        if (_isShooting)
        {
            _isShooting = Mouse.current.leftButton.isPressed;
        }

        // Handle shooting
        if (_isShooting)
        {
            HandleShoot();
        }
        
        // Calculate if the player is moving or is still
        _isMoving = _movement != Vector2.zero;

        if (_isMoving)
        {
            // Play the movement sound
            AudioManager.PlaySound(AudioManager.Sound.PlayerWalk, transform.position);
        }

        // See if the player is within the sunlight circle or box collider
        var tp = transform.position;
        inSunlight = Physics2D.OverlapCircle(tp, 0.5f, LayerMask.GetMask("Sunlight"))
                     || Physics2D.OverlapBox(tp, new Vector2(0.5f, 0.5f), 0, LayerMask.GetMask("Sunlight"));
        
        inSunlight = ConvertIfCorrupted(inSunlight);
    }
    #endregion
    

    public void TakeDamage(int damage)
    {
        _health.TakeDamage(damage);
    }

    public void PlayUpgradeAnimation()
    {
        _animator.SetTrigger(Upgrade);
        upgradeParticles.Play();
    }

    public void PlayDowngradeAnimation()
    {
        _animator.SetTrigger(Downgrade);
        downgradeParticles.Play();
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
        abilities.Add(ability);
        ability.Apply(gameObject);
    }

    public void RemoveAbility(AbilityEffect ability)
    {
        // Remove an ability from the player's abilities
        abilities.Remove(ability);
        
        // Remove the ability
        if (ability.abilityName == "Minimap")
        {
            MinimapManager.Instance.showMinimap = false;
        }
        else
        {
            ability.Unapply(gameObject);
        }
    }

    public bool HasAbility(AbilityEffect ability)
    {
        // Check if the player has an ability
        return abilities.Contains(ability);
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
        abilities.Clear();
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

    #endregion

    public void GoToSleep()
    {
        _animator.SetTrigger(Sleep);
        isSleeping = true;
    }
    
    public void WakeUp()
    {
        _animator.SetTrigger(Up);
        isSleeping = false;

        if (HomeRoomController.Instance == null || _hasWokenUp) return;
        HomeRoomController.Instance.NewDay();
        _hasWokenUp = true;
    }
    
    public void UnapplyAllAbilities()
    {
        // Unapply all abilities
        foreach (var ability in abilities)
        {
            ability.Unapply(gameObject);
        }
        
        // Clear the abilities
        abilities.Clear();
    }
    
    public void LoadData(GameData data)
    {
        //ONLY APPLICABLE IF CAN SAVE MID DUNGEON RUN
        // if (ScenesManager.Instance.currentSceneName == "ForestMain")
        // {
        //     foreach (var ability in data.Abilities)
        //     {
        //         var abilityEffect = AbilityManager.Instance.GetAbility(ability);
        //         abilityEffect.Apply(gameObject);
        //         
        //         AbilityManager.Instance.TriggerAbilityDisplay(abilityEffect);
        //     }
        // }

        // Load the essence
        essence = data.Essence;
        // Load the essence fragments
        essenceFragments = data.EssenceFragments;
    }

    public void SaveData(GameData data)
    {
        data.Abilities.Clear();
        // Save the player's position
        // data.PlayerPosition = transform.position;
        // Save each of the player's abilities
        foreach (var ability in abilities)
        {
            // Clear the abilities
            data.Abilities.Add(ability.name);
        }
        
        // Save the player's essence
        data.Essence = essence;
        // Save the player's essence fragments
        data.EssenceFragments = essenceFragments;
    }

    public bool FirstLoad()
    {
        return true;
    }
}