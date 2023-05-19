using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenesManager : MonoBehaviour
{
    // Make this a singleton
    public static ScenesManager Instance { get; private set; }
    
    // Keep track of the current scene
    public static string currentSceneName = "Home";

    private void Awake()
    {
        Instance = this;
    }
    
    public static void LoadScene(string sceneName)
    {
        // Stop the background music
        // Load the scene with the given name
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        // Set the current scene name to the given scene name
        currentSceneName = sceneName;
        
        // PLay the background music for the scene
        switch (sceneName)
        {
            case "Home":
                AudioManager.PlayBackgroundMusic(AudioManager.Sound.HomeBackgroundMusic);
                break;
            case "Forest":
                AudioManager.PlayBackgroundMusic(AudioManager.Sound.ForestBackgroundMusic);
                break;
        }
    }
}
