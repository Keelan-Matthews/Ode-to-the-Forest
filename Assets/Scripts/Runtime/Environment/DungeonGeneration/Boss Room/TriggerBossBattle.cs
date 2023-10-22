using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBossBattle : MonoBehaviour
{
    [SerializeField] private Room room;
    private bool _triggered;
    
    public static event Action OnStartBossBattle;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !_triggered)
        {
            _triggered = true;
            room.LockRoom();
            OnStartBossBattle?.Invoke();
        }
    }
}
