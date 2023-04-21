using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int speed = 5;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private float fireForce = 10f;
    [SerializeField] private float cooldownPeriod = 0.8f;
    public static PlayerController Instance;
    private bool _isScattershot = false;
    
    private Vector2 _movement;
    private Vector3 _mouseWorldPosition;
    private Rigidbody2D _rb;
    private Animator _animator;
    private Vector2 _smoothedMovement;
    private Vector2 _movementInputSmoothVelocity;
    private bool _canShoot = true;
    public bool inSunlight = false;
    private Health _health;
    
    // These are variables of things the player holds
    private int _essence = 0; // The currency of the game
    private List<AbilityEffect> _abilities = new (); // The abilities the player has equipped

    private void Awake()
    {
        Instance = this;
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();
    }
    
    private void OnMovement(InputValue value)
    {
        _movement = value.Get<Vector2>();
        
        // Update the animator with the new movement values so it can play the correct animation
        if (_movement.x != 0 || _movement.y != 0)
        {
            _animator.SetFloat("X", _movement.x);
            _animator.SetFloat("Y", _movement.y);
            
            _animator.SetBool("IsWalking", true); //Tell the animator that the player is moving
        } else
        {
            _animator.SetBool("IsWalking", false); //Tell the animator that the player is not moving
        }
    }

    private void OnShoot()
    {
        // Check if the player can shoot and if they are in the sunlight
        if (!_canShoot || !inSunlight) return;

        // Check if the player has the scattershot ability
        if (_isScattershot)
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

    private void Shoot()
    {
        // Get a bullet instance from the pool and set its position to the player's position
        var obj = ObjectPooler.Instance.GetPooledObject();
        if (obj == null) return;
        obj.transform.position = transform.position;
        obj.transform.rotation = transform.rotation;
        obj.SetActive(true);
        
        // Shoot the object in the direction of the mouse
        var direction = _mouseWorldPosition - transform.position;
        obj.GetComponent<Rigidbody2D>().velocity = new Vector2(direction.x, direction.y).normalized * fireForce;
    }

    private void Scattershot()
    {
        // Shoot a regular shoot, but also fire one bullet angled to the left of that bullet,
        // and another to the right of it. Therefore shooting 3 bullets at once
        
        for (var i = 0; i < 3; i++)
        {
            // Get a bullet instance from the pool and set its position to the player's position
            var obj = ObjectPooler.Instance.GetPooledObject();
            if (obj == null) return;
            obj.transform.position = transform.position;
            obj.transform.rotation = transform.rotation;
            obj.SetActive(true);
            
            // Shoot the object in the direction of the mouse
            var direction = _mouseWorldPosition - transform.position;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            angle += i == 0 ? 0 : i == 1 ? -10 : 10;
            var newDirection = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right;
            obj.GetComponent<Rigidbody2D>().velocity = new Vector2(newDirection.x, newDirection.y).normalized * fireForce;
        }
    }
    
    private IEnumerator Cooldown()
    {
        _canShoot = false;
        yield return new WaitForSeconds(cooldownPeriod);
        _canShoot = true;
    }
    
    private void OnMousePosition(InputValue value)
    {
        var mousePosition = value.Get<Vector2>();
        _mouseWorldPosition = sceneCamera.ScreenToWorldPoint(mousePosition);
        var direction = _mouseWorldPosition - transform.position;
        
        // Update the animator with the new movement values so it can play the correct animation
        _animator.SetFloat("X", direction.x);
        _animator.SetFloat("Y", direction.y);
    }

    private void FixedUpdate()
    {
        // Smooth the movement input
        _smoothedMovement = Vector2.SmoothDamp(_smoothedMovement, _movement, ref _movementInputSmoothVelocity, 0.1f);
        _rb.velocity = _smoothedMovement * speed;
    }

    public void TakeDamage(int damage)
    {
        _health.TakeDamage(damage);
    }

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
    
    public bool IsScattershot
    {
        get => _isScattershot;
        set => _isScattershot = value;
    }
    
    public void AddEssence(int amount)
    {
        // Add essence to the player's essence
        _essence += amount;
        
        // Update the UI
        GameManager.Instance.UpdateEssenceUI(_essence);
    }
    
    public void AddAbility(AbilityEffect ability)
    {
        // Add an ability to the player's abilities
        _abilities.Add(ability);
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
        return _essence;
    }
    
    public bool SpendEssence(int amount)
    {
        // Spend essence if the player has enough
        if (_essence < amount) return false;
        _essence -= amount;
        return true;
    }
}
