using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Vector2 _movement;
    private Rigidbody2D _rb;
    [SerializeField] private int speed = 5;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
    
    private void OnMovement(InputValue value)
    {
        _movement = value.Get<Vector2>();
    }
    
    private void FixedUpdate()
    {
        _rb.velocity = _movement * speed;
    }
}
