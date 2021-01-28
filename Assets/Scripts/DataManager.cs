using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class DataManager: MonoBehaviour
{
    public PlayerData data;
    private static string file = "PlayerData.json";
    private static string GameDataPath => Application.persistentDataPath; // @"D:\SimpaGameBotData";
    public string GameDataFile => Path.Combine(GameDataPath, file);

    public void Save()
    {
        // Debug.Log(data);
        string json = JsonConvert.SerializeObject(data);
        //WriteToFile(file, json);
        System.IO.File.WriteAllText(GameDataFile, json);
    }

    public void NewSave(Dictionary<string, PlayerData> gameData)
    {
        //Code saves at this point to our text file 
        string json = JsonConvert.SerializeObject(gameData, Formatting.Indented);
        System.IO.File.WriteAllText(GameDataFile, json);
    }

    public void Backup(Dictionary<string, PlayerData> gameData)
    {
        //Code saves at this point to our text file 
        // Debug.Log(data);
        string json = JsonConvert.SerializeObject(gameData, Formatting.Indented);
        System.IO.File.WriteAllText(GameDataFile, json);
    }

    public string Load()
    {
        data = new PlayerData();
        string json = ReadFromFile(GameDataFile);
        return json;
        //JsonUtility.FromJsonOverwrite(json, data);
    }

    public void WriteToFile(string fileName, string json)
    {
        // string path = GetFilePath(fileName);
        FileStream fileStream = new FileStream(fileName, FileMode.Create);
        using(StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(json);
        }
    }

    private string ReadFromFile(string fileName)
    {
        if (File.Exists(fileName))
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                string json = reader.ReadToEnd();
                return json;
            }
        }
        else
        {
            Debug.LogWarning("File not Found");
            return "";
        }
    }

}
