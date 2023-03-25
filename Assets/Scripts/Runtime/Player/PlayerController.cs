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
    [SerializeField] private float cooldownPeriod = 0.5f;
    
    private Vector2 _movement;
    private Vector3 _mouseWorldPosition;
    private Rigidbody2D _rb;
    private Animator _animator;
    private Vector2 _smoothedMovement;
    private Vector2 _movementInputSmoothVelocity;
    private bool _canShoot = true;
    public bool inSunlight = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
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

        // Get a bullet instance from the pool and set its position to the player's position
        var obj = ObjectPooler.Instance.GetPooledObject();
        if (obj == null) return;
        obj.transform.position = transform.position;
        obj.transform.rotation = transform.rotation;
        obj.SetActive(true);
        
        // Shoot the object in the direction of the mouse
        var direction = _mouseWorldPosition - transform.position;
        obj.GetComponent<Rigidbody2D>().velocity = new Vector2(direction.x, direction.y).normalized * fireForce;

        // Start the cooldown
        StartCoroutine(Cooldown());
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
}
