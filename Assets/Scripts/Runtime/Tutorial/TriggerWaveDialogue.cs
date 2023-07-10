using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerWaveDialogue : MonoBehaviour
{
    private bool _hasTriggered;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || _hasTriggered) return;
        
        TutorialManager.Instance.ResumeTutorial();
        
        // Make the player not invincible
        PlayerController.Instance.SetInvincible(false);
        
        _hasTriggered = true;
    }
}
