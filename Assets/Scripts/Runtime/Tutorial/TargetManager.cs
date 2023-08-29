using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    private int _targetsShot;
    private bool _cleared;
    private bool _hasEntered;
    
    public void IncrementTargetsShot()
    {
        _targetsShot++;
    }

    private void Update()
    {
        // If all targets have been shot and the room has not been cleared yet, clear the room
        if (_targetsShot != transform.childCount || _cleared) return;
        // Get the current room
        var currentRoom = GameManager.Instance.activeRoom;
        // Trigger the OnRoomClear event
        currentRoom.OnWaveEnd();
        currentRoom.UnlockRoom();
        _cleared = true;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Spawn the first enemy
        if (!other.CompareTag("Player") || _hasEntered) return;
        TutorialManager.Instance.ResumeTutorial();
        _hasEntered = true;
    }
}
