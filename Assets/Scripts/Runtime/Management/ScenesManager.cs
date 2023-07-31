using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour, IDataPersistence
{
    // Make this a singleton
    public static ScenesManager Instance { get; private set; }
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject menu;
    [SerializeField] private Slider loadingBar;
    
    [Header("Data Persistence")]
    [SerializeField] private bool firstLoad;
    
    // Keep track of the current scene
    public string currentSceneName;

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

    private void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
    }

    public static void LoadScene(string sceneName)
    {
        // Set the current scene name to the given scene name
        Instance.currentSceneName = sceneName;
        
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
        yield return new WaitForSeconds(1f);

        // PLay the background music for the scene
        // switch (sceneName)
        // {
        //     case "Home":
        //         AudioManager.PlayBackgroundMusic(AudioManager.Sound.HomeBackgroundMusic);
        //         break;
        //     case "Forest":
        //         AudioManager.PlayBackgroundMusic(AudioManager.Sound.ForestBackgroundMusic);
        //         break;
        // }

        // Hide the loading screen
        loadingScreen.SetActive(false);
    }

    public void LoadData(GameData data)
    {
        // // Only do so if it isn't the main menu
        // if (currentSceneName == "MainMenu") return;
        // Debug.Log($"Loading Data scene: {data.CurrentSceneName}");
        // currentSceneName = data.CurrentSceneName;
        
        // I should not load the current active scene
    }

    public void SaveData(GameData data)
    {
        // Only do so if it isn't the main menu
        if (currentSceneName == "MainMenu") return;
        data.CurrentSceneName = currentSceneName;
    }

    public bool FirstLoad()
    {
        return firstLoad;
    }
}