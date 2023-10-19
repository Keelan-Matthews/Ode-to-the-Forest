using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNewScene : MonoBehaviour
{
    public string sceneName;
    [SerializeField] private SeedPlotController specialPlot;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        // If the tutorial is active, return
        if (GameManager.Instance.isTutorial) return;
        // If the player has the Seed Of Life, return
        if (PermaSeedManager.Instance.HasSeed("Seed Of Life")) return;
        if (PermaSeedManager.Instance.HasSeed("Vase")) return;
        if (col.CompareTag("Player"))
        {
            if (specialPlot != null && !specialPlot.isLocked && specialPlot.isGrown)
            {
                GameManager.Instance.shouldWilt = true;
            }
            
            DataPersistenceManager.Instance.SaveGame();
            // Load the next scene
            ScenesManager.LoadScene(sceneName);
        }
    }
}
