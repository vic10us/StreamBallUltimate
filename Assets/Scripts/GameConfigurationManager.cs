using UnityEngine;
using SharpConfig;
using System.IO;
using System;

public class GameConfigurationManager: MonoBehaviour
{
    private string ConfigurationPath => Application.persistentDataPath;
    public string ConfigurationFileName = "global.cfg";
    private Configuration _config;
    private string ConfigurationFile => Path.Combine(ConfigurationPath, ConfigurationFileName);

    private void Start() {
        if (File.Exists(ConfigurationFile)) {
            _config = Configuration.LoadFromFile(ConfigurationFile);
        } else {
            _config = new Configuration();
            _config.SaveToFile(ConfigurationFile);
        }
        var c = GetGameConfiguration();
        c.LastRun = DateTimeOffset.UtcNow;
        SaveGameConfiguration(c);
        // lets do some config stuff
    }

    private void Save() {
        _config.SaveToFile(ConfigurationFile);
    }

    public void SaveGameConfiguration(GameConfiguration config) {
        var section = Section.FromObject("main", config);
        _config.RemoveAllNamed("main");
        _config.Add(section);
        Save();
    }

    public GameConfiguration GetGameConfiguration() {
        return _config["main"].ToObject<GameConfiguration>();
    }

}