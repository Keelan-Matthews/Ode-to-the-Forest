using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    private int _targetsShot;
    
    public void IncrementTargetsShot()
    {
        _targetsShot++;
    }

    private void Update()
    {
        // If all targets have been shot, trigger on room clear on GameManager
        if (_targetsShot != transform.childCount) return;
        // Get the current room
        var currentRoom = GameManager.Instance.activeRoom;
        // Trigger the OnRoomClear event
        currentRoom.OnWaveEnd();
    }
}
