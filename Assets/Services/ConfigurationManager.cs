using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class ConfigurationManager {

    public static T GetConfig<T>(string filename, bool autoCreate = false) {
        if (!File.Exists(filename)) {
            var x = (T)Activator.CreateInstance(typeof(T));
            if (autoCreate) SaveConfig(filename, x);
            return x;
        }
        var json = File.ReadAllText(filename);
        // var o = JsonUtility.FromJson<T>(json);
        var o = JsonConvert.DeserializeObject<T>(json);
        return o;
    }

    public static void SaveConfig(string filename, object o) {
        // var json = JsonUtility.ToJson(o);
        var json = JsonConvert.SerializeObject(o, Formatting.Indented);
        Debug.LogWarning(json);
        if (File.Exists(filename)) File.Delete(filename);
        Debug.LogWarning($"Saving config file: {filename}");
        File.WriteAllText(filename, json, System.Text.Encoding.UTF8);
    }

}