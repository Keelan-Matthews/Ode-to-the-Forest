using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Door : MonoBehaviour
{
    public enum DoorType
    {
        Left,
        Right,
        Top,
        Bottom
    }
    
    public DoorType doorType;
    private GameObject _player;
    private const float WidthOffset = 7.5f; // Change based on width of player
    private bool _locked;
    private Animator _animator;
    public Light2D doorLight;
    
    // This holds the different animators scripts for the door
    public RuntimeAnimatorController[] doorAnimators;

    private float _maxLightBrightness = 0.9f;

    [SerializeField] private GameObject sprite;
    private static readonly int IsLocked = Animator.StringToHash("IsLocked");

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _animator = sprite.GetComponent<Animator>();
    }
    
    // Teleport the player to the next room to overcome the door being blocked
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player") || _locked) return;
        var position = _player.transform.position;
        position = doorType switch
        {
            DoorType.Bottom => new Vector2(position.x, position.y - WidthOffset),
            DoorType.Left => new Vector2(position.x - WidthOffset, position.y),
            DoorType.Right => new Vector2(position.x + WidthOffset, position.y),
            DoorType.Top => new Vector2(position.x, position.y + WidthOffset),
            _ => position
        };
        _player.transform.position = position;
    }


    public void LockDoor()
    {
        _locked = true;
        _animator.SetBool(IsLocked, true);
        
        doorLight.intensity = 0f;
    }

    public void UnlockDoor()
    {
        _locked = false;
        _animator.SetBool(IsLocked, false);
        
        doorLight.intensity = _maxLightBrightness;
    }

    public void SetDoorType(string type)
    {
        Debug.Log("Setting door type to " + type);
        // get the door animator that matches the type
        for (var i = 0; i < doorAnimators.Length; i++)
        {
            // Check if the name of the animator contains the type
            if (!doorAnimators[i].name.Contains(type)) continue;
            Debug.Log("Found door animator " + doorAnimators[i].name);
            // Set the animator to the correct one
            _animator.runtimeAnimatorController = doorAnimators[i];
            break;
        }
    }
}
