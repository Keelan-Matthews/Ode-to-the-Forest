using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("Debugging")] [SerializeField] private bool disableDataPersistence;
    [SerializeField] private bool initializeDataIfNull;
    [SerializeField] private bool overrideSelectedProfileId;
    [SerializeField] private string testSelectedProfileId = "test";

    [Header("File Storage Config")] [SerializeField]
    private string fileName = "data.game";

    [SerializeField] private bool useEncryption;
    private GameData _gameData;
    private List<IDataPersistence> _dataPersistenceObjects;
    private FileDataHandler _dataHandler;
    private string _selectedProfileId = "test";
    public GameObject saveIcon;
    
    [Header("Data Persistence")]
    [SerializeField] private bool firstLoad;
    
    public static DataPersistenceManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate GameManager instances
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scene changes

        if (disableDataPersistence)
        {
            Debug.LogWarning("Data persistence is disabled.");
        }

        _dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);

        // Initialize selected profile id
        InitializeSelectedProfileId();
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
        _dataPersistenceObjects = FindAllDataPersistenceObjects();

        LoadGame();
    }
    
    public void RemoveDataPersistenceObject(IDataPersistence dataPersistenceObject)
    {
        if (_dataPersistenceObjects == null) return;
        _dataPersistenceObjects.Remove(dataPersistenceObject);
    }

    public void ChangeSelectedProfileId(string profileId)
    {
        _selectedProfileId = profileId;
        // Load the game
        LoadGame();
    }

    public void DeleteProfileData(string profileId)
    {
        _dataHandler.Delete(profileId);
        // Initialize selected profile id
        InitializeSelectedProfileId();
        // reload the game
        LoadGame();
    }

    private void InitializeSelectedProfileId()
    {
        _selectedProfileId = _dataHandler.GetLastPlayedProfileId();

        if (overrideSelectedProfileId)
        {
            _selectedProfileId = testSelectedProfileId;
            Debug.LogWarning($"Selected profile id is overridden to {_selectedProfileId}");
        }
    }
    
    public long GetLastUpdated()
    {
        var lastId = _dataHandler.GetLastPlayedProfileId();
        var data = _dataHandler.Load(lastId);
        return data.LastUpdated;
    }

    public void NewGame()
    {
        _gameData = new GameData();
    }

    public void LoadGame()
    {
        if (disableDataPersistence) return;
        // Load the game data
        _gameData = _dataHandler.Load(_selectedProfileId);

        // Debugging
        if (_gameData == null && initializeDataIfNull)
        {
            Debug.Log("No game data found. Initializing new game data.");
            NewGame();
        }

        // If no game data exists, create a new game
        if (_gameData == null)
        {
            Debug.Log("No game data found. A new game needs to be started before loading.");
            return;
        }

        foreach (var dataPersistenceObject in _dataPersistenceObjects)
        {
            dataPersistenceObject.LoadData(_gameData);
        }
    }

    public void SaveGame(bool hideIcon = false)
    {
        if (disableDataPersistence) return;

        // Check if the game data exists
        if (_gameData == null)
        {
            Debug.Log("No game data found. A new game needs to be started before saving.");
            return;
        }

        // Pass the data to each data persistence object
        foreach (var dataPersistenceObject in _dataPersistenceObjects)
        {
            // Only save if it is active 
            // if (!dataPersistenceObject.IsActive()) continue;
            
            dataPersistenceObject.SaveData(_gameData);
        }

        // timestamp the data
        _gameData.LastUpdated = DateTime.Now.ToBinary();
        
        // Save the game data
        _dataHandler.Save(_gameData, _selectedProfileId);

            if (hideIcon) return;
        // Start the coroutine to show the save icon
        StartCoroutine(ShowSaveIcon());
    }

    private IEnumerator ShowSaveIcon()
    {
        // Show the save icon
        saveIcon.SetActive(true);

        // Wait for 2 seconds using unscaled time
        float timer = 0f;
        float waitTime = 2f;
        while (timer < waitTime)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        // Hide the save icon
        saveIcon.SetActive(false);
    }

    private void OnApplicationQuit()
    {
        // If the current scene is not Home or Menu, return
        if (SceneManager.GetActiveScene().name != "Home" && SceneManager.GetActiveScene().name != "MainMenu") return;
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        var dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true).OfType<IDataPersistence>();
        
        // remove any items with a firstLoad flag set to false
        dataPersistenceObjects = dataPersistenceObjects.Where(x => x.FirstLoad());
        
        
        return dataPersistenceObjects.ToList();
    }

    public bool HasGameData()
    {
        return _gameData != null;
    }

    public Dictionary<string, GameData> GetAllProfilesGameData()
    {
        return _dataHandler.LoadAllProfiles();
    }

    public string GetLastScene()
    {
        return _gameData.CurrentSceneName;
    }
}