using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")] 
    [SerializeField] private string fileName = "data.game";
    [SerializeField] private bool useEncryption;
    private GameData _gameData;
    private List<IDataPersistence> _dataPersistenceObjects;
    private FileDataHandler _dataHandler;
    public static DataPersistenceManager Instance { get; private set; }
    
    private void Awake()
    {
        // if (Instance != null && Instance != this)
        // {
        //     Destroy(gameObject); // Destroy duplicate GameManager instances
        //     return;
        // }
        
        Instance = this;
        // DontDestroyOnLoad(gameObject); // Persist across scene changes
        
        _dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }
    
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    private void OnSceneUnloaded(Scene scene)
    {
        SaveGame();
    }

    public void NewGame()
    {
        _gameData = new GameData();
    }
    
    public void LoadGame()
    {
        // Load the game data
        _gameData = _dataHandler.Load();
        
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
    
    public void SaveGame()
    {
        // Check if the game data exists
        if (_gameData == null)
        {
            Debug.Log("No game data found. A new game needs to be started before saving.");
            return;
        }
        
        // Pass the data to each data persistence object
        foreach (var dataPersistenceObject in _dataPersistenceObjects)
        {
            dataPersistenceObject.SaveData(ref _gameData);
        }
        
        // Save the game data
        _dataHandler.Save(_gameData);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
    
    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        var dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return dataPersistenceObjects.ToList();
    }
    
    public bool HasGameData()
    {
        return _gameData != null;
    }
}
