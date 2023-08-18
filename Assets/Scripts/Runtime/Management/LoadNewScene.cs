using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNewScene : MonoBehaviour
{
    public string sceneName;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        // If the tutorial is active, return
        if (GameManager.Instance.isTutorial) return;
        if (col.CompareTag("Player"))
        {
            DataPersistenceManager.Instance.SaveGame();
            // Load the next scene
            ScenesManager.LoadScene(sceneName);
        }
    }
}
