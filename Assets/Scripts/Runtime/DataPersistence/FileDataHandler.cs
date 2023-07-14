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
    private const string EncryptionPassword = "SDF8fdn1~!@#2sdfL[]dfgk3$%4dfgHJK5&*()6dfg7DFG8dfg9DFG0dfg";
    private readonly string _backupExtension = ".bak";
    
    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        _dataDirPath = dataDirPath;
        _dataFileName = dataFileName;
        _useEncryption = useEncryption;
    }

    public GameData Load(string profileId, bool allowRestoreFromBackup = true)
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
                using var stream = new FileStream(fullPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                using var reader = new StreamReader(stream);
                dataToLoad = reader.ReadToEnd();
                reader.Close();

                // Decrypt the data if necessary
                if (_useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }

                // Deserialize the data from a JSON string
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                if (allowRestoreFromBackup)
                {
                    Debug.LogWarning($"Failed to load data file. Attempting to load backup file. Error: {e.Message}");
                    // Attempt to load the backup file
                    var rollBackSuccess = AttemptRollback(fullPath);
                    if (rollBackSuccess)
                    {
                        // try to load again recursively
                        loadedData = Load(profileId, false);
                    }
                }
                else
                {
                    Debug.LogError($"Failed to restore from backup file. Error: {e.Message}");
                }
            }
        }
        else
        {
            Debug.LogWarning($"No data file found at {fullPath}");
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
        var backupFilePath = fullPath + _backupExtension;
        
        try
        {
            // Create the directory if it doesn't exist
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ?? string.Empty);
            // Serialize the data to a JSON string
            var dataToStore = JsonUtility.ToJson(data, true); ;

            // Encrypt the data if necessary
            if (_useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }
            
            // Write the data to a file
            using var stream = new FileStream(fullPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            using var writer = new StreamWriter(stream);
            writer.Write(dataToStore);
            writer.Close();

            // Create a backup of the data file
            var verifiedGameData = Load(profileId);
            
            if (verifiedGameData != null)
            {
                File.Copy(fullPath, backupFilePath, true);
            }
            else
            {
                throw new Exception("Save file could not be verified and was not backed up");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving data to {fullPath}: {e.Message}");
        }
    }

    public void Delete(string profileId)
    {
        if (string.IsNullOrEmpty(profileId))
        {
            Debug.LogError("Profile ID is null or empty");
            return;
        }
        
        // Create the full path to the data file
        var fullPath = Path.Combine(_dataDirPath, profileId, _dataFileName);

        try
        {
            // Ensure the data file exists
            if (File.Exists(fullPath))
            {
                Directory.Delete(Path.GetDirectoryName(fullPath) ?? string.Empty, true);
            }
            else
            {
                Debug.LogWarning($"No data file found for profile {profileId}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error deleting data from {fullPath}: {e.Message}");
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

    private bool AttemptRollback(string fullPath)
    {
        var success = false;
        var backupFilePath = fullPath + _backupExtension;
        try
        {
            if (File.Exists(backupFilePath))
            {
                File.Copy(backupFilePath, fullPath, true);
                success = true;
                Debug.LogWarning($"Rolling back data file {fullPath}");
            }
            else
            {
                throw new Exception("No backup file found");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error rolling back data file {fullPath}: {e.Message}");
        }
        
        
        return success;
    }
}
