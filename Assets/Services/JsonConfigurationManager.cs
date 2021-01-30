#pragma warning disable 649
#pragma warning disable IDE0051 // Remove unused private members
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable IteratorNeverReturns

using System;
using System.IO;
using Newtonsoft.Json;

public static class JsonConfigurationManager {

    public static T GetConfig<T>(string filename, bool autoCreate = false) {
        if (!File.Exists(filename)) {
            var x = (T)Activator.CreateInstance(typeof(T));
            if (autoCreate) SaveConfig(filename, x);
            return x;
        }
        var json = File.ReadAllText(filename);
        var o = JsonConvert.DeserializeObject<T>(json);
        return o;
    }

    public static void SaveConfig(string filename, object o) {
        var json = JsonConvert.SerializeObject(o, Formatting.Indented);
        if (File.Exists(filename)) File.Delete(filename);
        File.WriteAllText(filename, json, System.Text.Encoding.UTF8);
    }

}