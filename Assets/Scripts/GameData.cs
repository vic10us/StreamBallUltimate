﻿#pragma warning disable 649
#pragma warning disable IDE0051 // Remove unused private members
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable IteratorNeverReturns

using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

public class GameData : MonoBehaviour
{
    //game data is in the form keys (unique ID) and data in the form of Player Data (see Player Data Script)
    private Dictionary<string, PlayerData> gameData = new Dictionary<string, PlayerData>();
    public DataManager dataManager;
    private readonly MarbleList marbleList;
    private readonly List<PlayerData> playerDataList;//GameData.values()
    private readonly List<string> gameDataKeyList;

    void Start()
    {
        //Load game data at start of game from JSON
        gameData = JsonConvert.DeserializeObject<Dictionary<string, PlayerData>>(dataManager.Load());
        foreach (var kvp in gameData) {
            if (string.IsNullOrWhiteSpace(kvp.Value.selectedSkin)) kvp.Value.selectedSkin = "default";
            if (!kvp.Value.purchasedSkins.Any()) kvp.Value.purchasedSkins.Add("default");
        }
        dataManager.NewSave(gameData);
    }

    public bool CheckIfPlayerExists(string playerID)
    {
        return gameData.ContainsKey(playerID);
    }

    public void CreateNewPlayerEntry(Arrrgs e)
    {
        var tempData = new PlayerData
        {
            money = 0, 
            selectedSkin = "default", 
            playerName = e.displayName, 
            isSubscribed = false
        };
        gameData.Add(e.userID, tempData);
        SaveGameDataToTXT();
    }

    public void AddMoneyToPlayerID(int money, string playerID)
    {
        var userGameData = gameData[playerID];
        if (userGameData == null) return;
        gameData[playerID].money += money;
        SaveGameDataToTXT();
    }

    public void SubtractMoneyFromPlayerID(int money, string playerID)
    {
        var userGameData = gameData[playerID];
        if (userGameData == null) return;
        gameData[playerID].money -= money;
        SaveGameDataToTXT();
    }

    public int CheckPlayerMoney(string playerID)
    {
        return gameData[playerID].money;
    }

    public bool IsSkinUnlocked(string playerId, string commonName) {
        return gameData[playerId]?
            .purchasedSkins
            .Any(c => c.Equals(commonName, System.StringComparison.InvariantCultureIgnoreCase)) 
            ?? false;
    }

    public void UnlockSkinForPlayer(string playerID, string commonName) {
        var playerData = gameData[playerID];
        if (!playerData.purchasedSkins.Any(s => s.Equals(commonName, System.StringComparison.InvariantCultureIgnoreCase)))
            playerData.purchasedSkins.Add(commonName);
        SaveGameDataToTXT();
    }

    public void SetPlayerEquipSkin(string playerID, string commonName)
    {
        var playerData = gameData[playerID];
        playerData.selectedSkin = commonName;
        SaveGameDataToTXT();
    }

    public string GetPlayerEquipSkin(string playerID)
    {
        var playerData = gameData[playerID];
        return string.IsNullOrWhiteSpace(playerData.selectedSkin) ? "default" : playerData.selectedSkin;
    }

    public void SaveGameDataToTXT()
    {
        dataManager.NewSave(gameData);
    }

    public string CheckSkins(Arrrgs e)
    {
        var playerID = e.userID;
        var playerName = e.displayName;
        var playerData = gameData[playerID];
        return $"@{playerName} owns [{string.Join(", ", playerData.purchasedSkins)}]";
    }
    
    public string ConvertCommonNameToUserID(string commonName)
    {
        var e = gameData.FirstOrDefault(kvp => kvp.Value.playerName.Equals(commonName, System.StringComparison.InvariantCultureIgnoreCase));
        return e.Key ?? "";
    }
    
    public bool CheckPlayerIDMatchesUserName(string userID, string name)
    {
        return gameData[userID].playerName == name;
    }

}
