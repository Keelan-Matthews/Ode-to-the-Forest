using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

public class JsonDataService : IDataService
{
    // Encryption and decryption key and initialization vector
    private const string KEY = "ggdPhkeOoiv6YMiPWa34kIuOdDUL7NwQFg6l1DVdwN8=";
    private const string IV = "JZuM0HQsWSBVpRHTeRZMYQ==";
    
    public bool SaveData<T>(string relativePath, T data, bool encrypted)
    {
        // Get the path to the file
        var path = Application.persistentDataPath + relativePath;

        try
        {
            if (UnityEngine.Windows.File.Exists(path))
            {
                Debug.Log("Data exists. Deleting old file and writing a new one.");
                File.Delete(path);
            }
            else
            {
                Debug.Log("Data does not exist. Writing a new file.");
            }
            
            using var stream = File.Create(path);
            
            // Check if the data should be encrypted
            if (encrypted)
            {
                WriteEncryptedData(data, stream);
            }
            else
            {
                stream.Close();
                File.WriteAllText(path, JsonConvert.SerializeObject(data));
            }
            return true;
        }
        catch (IOException e)
        {
            Debug.Log($"Unable to save data due to {e.Message} at {e.StackTrace}");
            throw;
        }
    }

    private void WriteEncryptedData<T>(T data, FileStream stream)
    {
        using var aesProvider = Aes.Create();
        aesProvider.Key = Convert.FromBase64String(KEY);
        aesProvider.IV = Convert.FromBase64String(IV);
        using var cryptoTransform = aesProvider.CreateEncryptor();
        using var cryptoStream = new CryptoStream(
            stream, 
            cryptoTransform, 
            CryptoStreamMode.Write
            );
        
        cryptoStream.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(data)));
    }

    public T LoadData<T>(string relativePath, bool encrypted)
    {
        var path = Application.persistentDataPath + relativePath;

        if (!File.Exists(path))
        {
            Debug.Log($"Cannot load file at {path} because it does not exist.");
            throw new FileNotFoundException($"{path} does not exist.");
        }
        
        try
        {
            T data;
            if (encrypted)
            {
                data = ReadEncryptedData<T>(path);
            }
            else
            {
                data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            }
            return data;
        }
        catch (IOException e)
        {
            Debug.Log($"Unable to load data due to {e.Message} at {e.StackTrace}");
            throw;
        }
    }
    
    private T ReadEncryptedData<T>(string path)
    {
        var fileBytes = File.ReadAllBytes(path);
        using var aesProvider = Aes.Create();
        aesProvider.Key = Convert.FromBase64String(KEY);
        aesProvider.IV = Convert.FromBase64String(IV);
        using var cryptoTransform = aesProvider.CreateDecryptor(
            aesProvider.Key,
            aesProvider.IV
            );
        
        using var memoryStream = new MemoryStream(fileBytes);
        using var cryptoStream = new CryptoStream(
            memoryStream,
            cryptoTransform,
            CryptoStreamMode.Read
            );
        
        using var streamReader = new StreamReader(cryptoStream);
        var decryptedData = streamReader.ReadToEnd();
        return JsonConvert.DeserializeObject<T>(decryptedData);
    }
}