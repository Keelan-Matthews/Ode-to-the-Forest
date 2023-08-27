using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class ScenesManager : MonoBehaviour, IDataPersistence
{
    // Make this a singleton
    public static ScenesManager Instance { get; private set; }
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject menu;
    [SerializeField] private Slider loadingBar;
    private static bool _isLoading;
    
    [Header("Loading screen tips")]
    [SerializeField] private List<string> toHomeTips;
    [SerializeField] private List<string> toForestTips;
    [SerializeField] private List<string> afterDeathTips;
    [SerializeField] private TextMeshProUGUI tipText;
    
    [Header("Loading screen info")]
    [SerializeField] private string toForestInfo;
    [SerializeField] private string toHomeInfo;
    [SerializeField] private string afterDeathInfo;
    [SerializeField] private string toMainMenuInfo;
    [SerializeField] private TextMeshProUGUI infoText;

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
        _isLoading = true;
        Instance.StartCoroutine(Instance.LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        var loadOperation = SceneManager.LoadSceneAsync(sceneName);

        // Show a random tip based on the scene we're loading
        switch (sceneName)
        {
            case "MainMenu":
                tipText.text = toHomeTips[Random.Range(0, toHomeTips.Count)];
                infoText.text = toMainMenuInfo;
                break;
            case "ForestMain":
                tipText.text = toForestTips[Random.Range(0, toForestTips.Count)];
                infoText.text = toForestInfo;
                break;
            case "Tutorial":
                tipText.text = "Listen to Mother, she will guide you.";
                infoText.text = "Loading tutorial...";
                break;
            case "Home":
                // If the current scene is ForestMain, show afterDeathTips, else show toHomeTips
                tipText.text = currentSceneName == "ForestMain" ? afterDeathTips[Random.Range(0, afterDeathTips.Count)] : toHomeTips[Random.Range(0, toHomeTips.Count)];
                infoText.text = currentSceneName == "ForestMain" ? afterDeathInfo : toHomeInfo;
                break;
        }
        
        // Force canvas update
        Canvas.ForceUpdateCanvases();
        tipText.GetComponentInParent<HorizontalLayoutGroup>().enabled = false;
        tipText.GetComponentInParent<HorizontalLayoutGroup>().enabled = true;

        // While the scene is loading
        while (!loadOperation.isDone)
        {
            var progress = Mathf.Clamp01(loadOperation.progress / 0.6f);
            loadingBar.value = progress * 0.6f; // Scale the progress to only reach 50%

            yield return null;
        }

        // Wait for an extra 2 seconds so that the player really feels like the game is loading
        StartCoroutine(LoadSecondHalf());
    }

    private IEnumerator LoadSecondHalf()
    {
        // Start from the current loadingBar value (should be around 0.5) and move to 0.9 over 2 seconds
        var startValue = loadingBar.value;
        var targetValue = 1f;
        var time = 0f;
    
        while (time < 2f)
        {
            time += Time.deltaTime;
            loadingBar.value = Mathf.Lerp(startValue, targetValue, time / 2f);
            yield return null;
        }
    
        // Hide the loading screen
        loadingScreen.SetActive(false);
        
        _isLoading = false;
    }
    
    public bool IsLoading()
    {
        return _isLoading;
    }

    public void LoadData(GameData data)
    {
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