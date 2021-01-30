#pragma warning disable 649
#pragma warning disable IDE0051 // Remove unused private members
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable IteratorNeverReturns

using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class DataManager: MonoBehaviour
{
    public PlayerData data;
    private const string file = "PlayerData.json";
    private static string GameDataPath => Application.persistentDataPath; // @"D:\SimpaGameBotData";
    public string GameDataFile => Path.Combine(GameDataPath, file);

    public void Save()
    {
        // Debug.Log(data);
        var json = JsonConvert.SerializeObject(data);
        //WriteToFile(file, json);
        File.WriteAllText(GameDataFile, json);
    }

    public void NewSave(Dictionary<string, PlayerData> gameData)
    {
        //Code saves at this point to our text file 
        var json = JsonConvert.SerializeObject(gameData, Formatting.Indented);
        File.WriteAllText(GameDataFile, json);
    }

    public void Backup(Dictionary<string, PlayerData> gameData)
    {
        //Code saves at this point to our text file 
        // Debug.Log(data);
        var json = JsonConvert.SerializeObject(gameData, Formatting.Indented);
        File.WriteAllText(GameDataFile, json);
    }

    public string Load()
    {
        data = new PlayerData();
        var json = ReadFromFile(GameDataFile);
        return json;
        //JsonUtility.FromJsonOverwrite(json, data);
    }

    public void WriteToFile(string fileName, string json)
    {
        // string path = GetFilePath(fileName);
        var fileStream = new FileStream(fileName, FileMode.Create);
        using (var writer = new StreamWriter(fileStream))
        {
            writer.Write(json);
        }
    }

    private static string ReadFromFile(string fileName)
    {
        if (!File.Exists(fileName))
        {
            Debug.LogWarning("File not Found");
            return string.Empty;
        }

        using (var reader = new StreamReader(fileName))
        {
            var json = reader.ReadToEnd();
            return json;
        }
    }

}
