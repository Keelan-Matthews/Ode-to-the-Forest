using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileDataHandler
{
    private readonly string _dataDirPath = "";
    private readonly string _dataFileName = "";
    private bool _useEncryption = false;
    // This is the password used to encrypt and decrypt the data and is a 32-byte string of random characters
    private readonly string _encryptionPassword = "SDF8fdn1~!@#2sdfL[]dfgk3$%^4dfgHJK5&*()6dfg7DFG8dfg9DFG0dfg";
    
    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        this._dataDirPath = dataDirPath;
        this._dataFileName = dataFileName;
        _useEncryption = useEncryption;
    }

    public GameData Load()
    {
        // Create the full path to the data file
        var fullPath = Path.Combine(_dataDirPath, _dataFileName);
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
    
    public void Save(GameData data)
    {
        // Create the full path to the data file
        var fullPath = Path.Combine(_dataDirPath, _dataFileName);
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
    
    private string EncryptDecrypt(string data)
    {
        var modifiedData = "";

        for (var i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ _encryptionPassword[i % _encryptionPassword.Length]);
        }
        
        return modifiedData;
    }
}
