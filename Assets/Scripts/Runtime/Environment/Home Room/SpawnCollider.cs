using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCollider : MonoBehaviour
{
    private float _sleepTime = 2f;
    private float _sleepTimer;
    private bool _isColliding;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _isColliding = true;
            // If Ode is sleeping, return
            if (PlayerController.Instance.isSleeping) return;
            
            // Wait for 4 seconds, and if the player is still in the collider, make sleep
            StartCoroutine(WaitForSleep());
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _isColliding = false;
            _sleepTimer = 0f;
        }
    }
    
    private IEnumerator WaitForSleep()
    {
        while (_sleepTimer < _sleepTime)
        {
            _sleepTimer += Time.deltaTime;
            yield return null;
        }
        
        // If still in the collider, make sleep
        if (!_isColliding) yield break;
        PlayerController.Instance.GoToSleep();
    }
}
