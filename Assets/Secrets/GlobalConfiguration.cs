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

    static string Or(this string value, string alternative) {
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
            Debug.Log("Config file found. [Loading]");
            _config = Configuration.LoadFromFile(ConfigurationFile);
        } else {
            Debug.Log("Config file NOT found. [Creating]");
            _config = new Configuration();
            _config.SaveToFile(ConfigurationFile);
        }

        bot_name = GetValue("twitch", "bot_name");
        client_id = GetValue("twitch", "client_id");
        client_secret = GetValue("twitch", "client_secret");
        bot_access_token = GetValue("twitch", "bot_access_token");
        bot_refresh_token = GetValue("twitch", "bot_refresh_token");
        channel_name = GetValue("twitch", "channel_name");

        /*
        string value;
        bool toDelete = false;

        // Check whether the environment variable exists.
        value = Environment.GetEnvironmentVariable("client_id");
        // If necessary, create it.
        if (value == null)
        {
            Environment.SetEnvironmentVariable("client_id", "");
            Environment.SetEnvironmentVariable("client_secret", "");
            Environment.SetEnvironmentVariable("bot_access_token", "");
            Environment.SetEnvironmentVariable("bot_refresh_token", "");
            toDelete = true;

            // Now retrieve it.
            client_id = Environment.GetEnvironmentVariable("client_id");
            client_secret = Environment.GetEnvironmentVariable("client_secret");
            bot_access_token = Environment.GetEnvironmentVariable("bot_access_token");
            bot_refresh_token = Environment.GetEnvironmentVariable("bot_refresh_token");
   */
    }
}
