using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileDataHandler
{
    private readonly string _dataDirPath;
    private readonly string _dataFileName;
    private readonly bool _useEncryption;
    // This is the password used to encrypt and decrypt the data and is a 32-byte string of random characters
    private const string EncryptionPassword = "SDF8fdn1~!@#2sdfL[]dfgk3$%^4dfgHJK5&*()6dfg7DFG8dfg9DFG0dfg";

    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        _dataDirPath = dataDirPath;
        _dataFileName = dataFileName;
        _useEncryption = useEncryption;
    }

    public GameData Load(string profileId)
    {
        // Base case if profile ID is null
        if (string.IsNullOrEmpty(profileId))
        {
            Debug.LogError("Profile ID is null or empty");
            return null;
        }
        // Create the full path to the data file
        var fullPath = Path.Combine(_dataDirPath, profileId, _dataFileName);
        GameData loadedData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                var dataToLoad = "";
                using var stream = new FileStream(fullPath, FileMode.Open);
                using var reader = new StreamReader(stream);
                dataToLoad = reader.ReadToEnd();
                
                // Decrypt the data if necessary
                if (_useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }
                
                // Deserialize the data from a JSON string
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error loading data from {fullPath}: {e.Message}");
            }
        }

        return loadedData;
    }
    
    public void Save(GameData data, string profileId)
    {
        // Base case if profile ID is null
        if (string.IsNullOrEmpty(profileId))
        {
            Debug.LogError("Profile ID is null or empty");
            return;
        }
        
        // Create the full path to the data file
        var fullPath = Path.Combine(_dataDirPath, profileId, _dataFileName);
        try
        {
            // Create the directory if it doesn't exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ?? string.Empty);
            // Serialize the data to a JSON string
            var dataToStore = JsonUtility.ToJson(data, true);
            
            // Encrypt the data if necessary
            if (_useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }
            // Write the data to a file
            using var stream = new FileStream(fullPath, FileMode.Create);
            using var writer = new StreamWriter(stream);
            writer.Write(dataToStore);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error saving data to {fullPath}: {e.Message}");
        }
    }

    public Dictionary<string, GameData> LoadAllProfiles()
    {
        var profileDictionary = new Dictionary<string, GameData>();
        
        // Loop through all the directories in the data directory
        var dirInfos = new DirectoryInfo(_dataDirPath).EnumerateDirectories();
        foreach (var dirInfo in dirInfos)
        {
            var profileId = dirInfo.Name;
            
            // Check if the profile has a data file
            var fullPath = Path.Combine(_dataDirPath, profileId, _dataFileName);
            if (!File.Exists(fullPath))
            {
                Debug.LogWarning($"No data file found for profile {profileId}");
                continue;
            }
            
            // Load the data file
            var profileData = Load(profileId);
            
            if (profileData != null)
            {
                profileDictionary.Add(profileId, profileData);
            }
            else
            {
                Debug.LogError($"Unable to load data for profile {profileId}");
            }
        }

        return profileDictionary;
    }

    public string GetLastPlayedProfileId()
    {
        string mostRecentProfileId = null;
        
        var profilesGameData = LoadAllProfiles();
        foreach (var pair in profilesGameData)
        {
            var profileId = pair.Key;
            var gameData = pair.Value;
            
            if (mostRecentProfileId == null)
            {
                mostRecentProfileId = profileId;
            }
            else
            {
                var mostRecentDateTime = DateTime.FromBinary(profilesGameData[mostRecentProfileId].LastUpdated);
                var currentDateTime = DateTime.FromBinary(gameData.LastUpdated);
                
                if (currentDateTime > mostRecentDateTime)
                {
                    mostRecentProfileId = profileId;
                }
            }
        }
        
        return mostRecentProfileId;
    }
    private string EncryptDecrypt(string data)
    {
        var modifiedData = "";

        for (var i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ EncryptionPassword[i % EncryptionPassword.Length]);
        }
        
        return modifiedData;
    }
}
