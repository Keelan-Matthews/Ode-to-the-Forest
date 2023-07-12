using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    // Make this a singleton
    public static ScenesManager Instance { get; private set; }
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject menu;
    [SerializeField] private Slider loadingBar;

    // Keep track of the current scene
    public string currentSceneName = "Home";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate GameManager instances
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scene changes
    }

    public static void LoadScene(string sceneName)
    {
        // Show the loading screen and hide the menu
        Instance.loadingScreen.SetActive(true);
        Instance.menu.SetActive(false);

        // Load the scene asynchronously
        Instance.StartCoroutine(Instance.LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        var loadOperation = SceneManager.LoadSceneAsync(sceneName);

        // While the scene is loading
        while (!loadOperation.isDone)
        {
            var progress = Mathf.Clamp01(loadOperation.progress / 0.9f);
            loadingBar.value = progress;

            yield return null;
        }
        
        // Wait for an extra 2 seconds so that the player really feels like the game is loading
        yield return new WaitForSeconds(2f);

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

        // Hide the loading screen
        loadingScreen.SetActive(false);

        // If the scene is the home scene, show the menu
        if (sceneName == "Home")
        {
            menu.SetActive(true);
        }
    }
}