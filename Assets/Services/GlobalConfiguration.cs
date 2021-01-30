#pragma warning disable 649
#pragma warning disable IDE0051 // Remove unused private members
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable IteratorNeverReturns

using System;
using System.IO;
using SharpConfig;
using UnityEngine;

public static class GlobalConfiguration
{
    public static string bot_name;
    public static string client_id;
    public static string client_secret; 
    public static string bot_access_token; 
    public static string bot_refresh_token; //hai
    public static string channel_name;
    private static string ConfigurationPath => Application.persistentDataPath;
    public static string ConfigurationFileName = "global.cfg";
    private static Configuration _config;
    private static string ConfigurationFile => Path.Combine(ConfigurationPath, ConfigurationFileName);

    public static string GetValue(string scope, string key) {
        var result = (_config[scope][key]?.StringValue ?? "").Or(GetEnvironmentVariable($"sbu_{scope}_{key}"));
        return result; // _config[scope][key]?.StringValue ?? GetEnvironmentVariable(key);
    }

    private static string Or(this string value, string alternative) {
        return string.IsNullOrWhiteSpace(value) ? alternative : value;
    }

    public static string GetEnvironmentVariable(string name)
    {
        var envVar = name.Replace(' ', '_');
        return Environment.GetEnvironmentVariable(envVar, EnvironmentVariableTarget.Process) ??
               Environment.GetEnvironmentVariable(envVar, EnvironmentVariableTarget.User) ??
               Environment.GetEnvironmentVariable(envVar, EnvironmentVariableTarget.Machine);
    }

    public static void LoadConfiguration()
    {
        if (File.Exists(ConfigurationFile)) {
            _config = Configuration.LoadFromFile(ConfigurationFile);
        } else {
            _config = new Configuration();
            _config.SaveToFile(ConfigurationFile);
        }

        bot_name = GetValue("twitch", "bot_name");
        client_id = GetValue("twitch", "client_id");
        client_secret = GetValue("twitch", "client_secret");
        bot_access_token = GetValue("twitch", "bot_access_token");
        bot_refresh_token = GetValue("twitch", "bot_refresh_token");
        channel_name = GetValue("twitch", "channel_name");
    }
}
