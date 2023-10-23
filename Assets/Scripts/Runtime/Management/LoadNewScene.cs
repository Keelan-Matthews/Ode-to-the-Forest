using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            if (specialPlot != null && !specialPlot.isLocked)
            {
                GameManager.Instance.shouldWilt = false;
            }
            
            // If all 3 seed plots have seeds, and at least one is rare, trigger vase spawn
            if (PermaSeedManager.Instance.GetActiveSeeds().Count >= 3
                && PermaSeedManager.Instance.GetActiveSeeds().Any(seed => seed.essenceRequired >= 10)
                && specialPlot.isLocked)
            {
                GameManager.Instance.CanSpawnVase = true;
            }

            if (!specialPlot.isLocked && HomeRoomController.Instance.GetEssence() >= 45)
            {
                GameManager.Instance.spawnOracle = true;
            }
            
            DataPersistenceManager.Instance.SaveGame();
            // Load the next scene
            ScenesManager.LoadScene(sceneName);
        }
    }
}
