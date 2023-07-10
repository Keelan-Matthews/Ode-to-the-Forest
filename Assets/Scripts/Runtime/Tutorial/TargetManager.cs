using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    private int _targetsShot;
    private bool _cleared;
    
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
        _cleared = true;
    }
}
