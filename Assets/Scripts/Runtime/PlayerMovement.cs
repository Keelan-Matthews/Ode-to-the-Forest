using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private int speed = 5;
    
    private Vector2 _movement;
    private Rigidbody2D _rb;
    private Animator _animator;
    
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
    
    private void FixedUpdate()
    {
        _rb.velocity = _movement * speed;
    }
}
