#pragma warning disable 649
#pragma warning disable IDE0051 // Remove unused private members
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable IteratorNeverReturns

using System;
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
            if (string.IsNullOrWhiteSpace(kvp.Value.SelectedSkin)) kvp.Value.SelectedSkin = "default";
            if (!kvp.Value.PurchasedSkins.Any()) kvp.Value.PurchasedSkins.Add("default");
        }
        dataManager.NewSave(gameData);
    }

    public void RenameAllPlayerSkins(string oldName, string newName)
    {
        var playersAffected = 
            gameData
                .Where(kvp => 
                    kvp.Value.PurchasedSkins.Any(sn => sn.Equals(oldName, StringComparison.InvariantCultureIgnoreCase)))
                .ToDictionary(k => k.Key, k => k.Value);
        if (!playersAffected.Any()) return;
        foreach (var p in playersAffected.Select(keyValuePair => gameData[keyValuePair.Key]))
        {
            if (p.SelectedSkin.Equals(oldName, StringComparison.InvariantCultureIgnoreCase))
                p.SelectedSkin = newName;
            p.PurchasedSkins = p.PurchasedSkins.Select(s => s.Equals(oldName, StringComparison.InvariantCultureIgnoreCase) ? newName : s).ToList();
        }
        SaveGameDataToTXT();
    }

    public bool CheckIfPlayerExists(string playerID)
    {
        return gameData.ContainsKey(playerID);
    }

    public void CreateNewPlayerEntry(Arrrgs e)
    {
        var tempData = new PlayerData
        {
            Money = 0, 
            SelectedSkin = "default", 
            PlayerName = e.DisplayName, 
            IsSubscribed = false
        };
        gameData.Add(e.UserID, tempData);
        SaveGameDataToTXT();
    }

    public void AddMoneyToPlayerID(int money, string playerID)
    {
        var userGameData = gameData[playerID];
        if (userGameData == null) return;
        gameData[playerID].Money += money;
        SaveGameDataToTXT();
    }

    public void SubtractMoneyFromPlayerID(int money, string playerID)
    {
        var userGameData = gameData[playerID];
        if (userGameData == null) return;
        gameData[playerID].Money -= money;
        SaveGameDataToTXT();
    }

    public int CheckPlayerMoney(string playerID)
    {
        return gameData[playerID].Money;
    }

    public bool IsSkinUnlocked(string playerId, string commonName) {
        return gameData[playerId]?
            .PurchasedSkins
            .Any(c => c.Equals(commonName, StringComparison.InvariantCultureIgnoreCase)) 
            ?? false;
    }

    public void UnlockSkinForPlayer(string playerID, string commonName) {
        var playerData = gameData[playerID];
        if (!playerData.PurchasedSkins.Any(s => s.Equals(commonName, StringComparison.InvariantCultureIgnoreCase)))
            playerData.PurchasedSkins.Add(commonName);
        SaveGameDataToTXT();
    }

    public void SetPlayerEquipSkin(string playerID, string commonName)
    {
        var playerData = gameData[playerID];
        playerData.SelectedSkin = commonName;
        SaveGameDataToTXT();
    }

    public string GetPlayerEquipSkin(string playerID)
    {
        var playerData = gameData[playerID];
        return string.IsNullOrWhiteSpace(playerData.SelectedSkin) ? "default" : playerData.SelectedSkin;
    }

    public void SaveGameDataToTXT()
    {
        dataManager.NewSave(gameData);
    }

    public string CheckSkins(Arrrgs e)
    {
        var playerID = e.UserID;
        var playerName = e.DisplayName;
        var playerData = gameData[playerID];
        return $"@{playerName} owns [{string.Join(", ", playerData.PurchasedSkins)}]";
    }
    
    public string ConvertCommonNameToUserID(string commonName)
    {
        var e = gameData.FirstOrDefault(kvp => kvp.Value.PlayerName.Equals(commonName, StringComparison.InvariantCultureIgnoreCase));
        return e.Key ?? "";
    }
    
    public bool CheckPlayerIDMatchesUserName(string userID, string name)
    {
        return gameData[userID].PlayerName == name;
    }

    internal string RemoveSkinFromPlayer(string playerID, string commonName)
    {
        var playerData = gameData[playerID];
        var skin = playerData.PurchasedSkins.FirstOrDefault(
            s => s.Equals(commonName, StringComparison.InvariantCultureIgnoreCase));
        if (string.IsNullOrWhiteSpace(skin)) return commonName;
        playerData.PurchasedSkins.Remove(skin);
        if (playerData.SelectedSkin.Equals(commonName, StringComparison.InvariantCultureIgnoreCase))
            playerData.SelectedSkin = playerData.PurchasedSkins.FirstOrDefault() ?? "default";
        SaveGameDataToTXT();
        return playerData.SelectedSkin;
    }
}
