using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsWalkingCollider : MonoBehaviour
{
    private void Start()
    {
        // Lock the doors after 1 second
        Invoke(nameof(LockRoom), 0.3f);
    }
    
    private void LockRoom()
    {
        // Get the current room
        var currentRoom = GameManager.Instance.activeRoom;
        // Lock the doors
        currentRoom.LockRoom();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        // Get the current room
        var currentRoom = GameManager.Instance.activeRoom;
        // Trigger the OnRoomClear event
        currentRoom.UnlockRoom();
        currentRoom.OnWaveEnd();
    }
}
