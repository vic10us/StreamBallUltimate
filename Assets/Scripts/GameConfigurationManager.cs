#pragma warning disable 649
#pragma warning disable IDE0051 // Remove unused private members
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable IteratorNeverReturns

using UnityEngine;
using SharpConfig;
using System.IO;
using System;

public class GameConfigurationManager: MonoBehaviour
{
    private static string ConfigurationPath => Application.persistentDataPath;
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

#if !UNITY_EDITOR   // Change the camera background only if we are not in the editor
        // ColorUtility.TryParseHtmlString("#47474700", out var color);
        ColorUtility.TryParseHtmlString("#FF00FF00", out var color);
        Camera.main.backgroundColor = color;
#endif
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