using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IsWalkingCollider : MonoBehaviour
{
    private bool _activated;
    private void Start()
    {
        PlayerController.Instance.GoToSleep();
        // Lock the doors after 1 second
        Invoke(nameof(LockRoom), 0.3f);
    }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Wait one second before starting the tutorial
        StartCoroutine(StartTutorial());
    }
    
    private IEnumerator StartTutorial()
    {
        yield return new WaitForSeconds(2f);
        TutorialManager.Instance.StartTutorial();
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
        if (!other.CompareTag("Player") || _activated) return;
        _activated = true;
        // Get the current room
        var currentRoom = GameManager.Instance.activeRoom;
        // Trigger the OnRoomClear event
        currentRoom.UnlockRoom();
        currentRoom.OnWaveEnd();
    }
}
