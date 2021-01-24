using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using TwitchLib.Client.Models;
using TwitchLib.Unity;
using TwitchLib.Client.Events;
using System.Linq;

public class GameData : MonoBehaviour
{
    //game data is in the form keys (unique ID) and data in the form of Player Data (see Player Data Script)
    Dictionary<string, PlayerData> gameData = new Dictionary<string, PlayerData>();
    public DataManager dataManager;
    MarbleList marbleList;
    List<PlayerData> playerDataList;//GameData.values()
    List<string> gameDataKeyList;

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
        Debug.Log($"Checking if player with ID [{playerID}] exists in config");
        if (gameData.ContainsKey(playerID) )
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void CreateNewPlayerEntry(Arrrgs e)
    {
        var marbleList = FindObjectOfType<MarbleList>();
        PlayerData tempData = new PlayerData();
        tempData.money = 0;
        tempData.selectedSkin = marbleList.marbleCodeToCommonName[0];
        tempData.playerName = e.displayName;
        tempData.isSubscribed = false;
        gameData.Add(e.userID, tempData);
        SaveGameDataToTXT();
        Debug.Log("GAME DATA SUCCESFULLY SAVED!!!!");
    }

    public void AddMoneyToPlayerID(int money, string playerID)
    {
        Debug.Log($"AddMoneyToPlayerID: {money} -> {playerID}");
        var userGameData = gameData[playerID];
        if (userGameData == null) return;
        gameData[playerID].money += money;
        SaveGameDataToTXT();
        Debug.Log("GAME DATA SUCCESFULLY SAVED!!!!");
    }

    public void SubtractMoneyFromPlayerID(int money, string playerID)
    {
        Debug.Log($"SubtractMoneyFromPlayerID: {money} -> {playerID}");
        var userGameData = gameData[playerID];
        if (userGameData == null) return;
        gameData[playerID].money -= money;
        SaveGameDataToTXT();
        Debug.Log("GAME DATA SUCCESFULLY SAVED!!!!");
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
        var marbleList = FindObjectOfType<MarbleList>();
        var skinIndex = marbleList.GetMarbleCodeFromCommonName(commonName);
        var playerData = gameData[playerID];
        if (!playerData.purchasedSkins.Any(s => s.Equals(commonName, System.StringComparison.InvariantCultureIgnoreCase)))
            playerData.purchasedSkins.Add(commonName);
        SaveGameDataToTXT();
        Debug.Log("GAME DATA SUCCESFULLY SAVED!!!!");
    }

    public void SetPlayerEquipSkin(string playerID, string commonName)
    {
        var playerData = gameData[playerID];
        playerData.selectedSkin = commonName;
        SaveGameDataToTXT();
        Debug.Log("GAME DATA SUCCESFULLY SAVED!!!!");
    }

    public string GetPlayerEquipSkin(string playerID)
    {
        var playerData = gameData[playerID];
        if (string.IsNullOrWhiteSpace(playerData.selectedSkin))
            return "default";
        return playerData.selectedSkin;
    }

    public void SaveGameDataToTXT()
    {
        dataManager.NewSave(gameData);
        Debug.Log("GAME DATA SUCCESSFULLY SAVED!!!!");
    }

    public bool CheckIfPlayerSubscribedToWhispers(string playerID)
    {
        return gameData[playerID].isSubscribed;
        //Returns true if player subscribed 
    }

    public string CheckSkins(Arrrgs e)
    {
        string playerID = e.userID;
        string playerName = e.displayName;
        var playerData = gameData[playerID];
        return $"{playerName}: {string.Join(", ", playerData.purchasedSkins)}";
    }
    
    public string ConvertCommonNameToUserID(string commonName)
    {
        //var myKey = dictionary.FirstOrDefault(x => x.Value == "one").Key;
        playerDataList = new List<PlayerData>(gameData.Values);
        for (int i = 0; i < playerDataList.Count; i++)
        {
            if (playerDataList[i].playerName.Equals(commonName,System.StringComparison.CurrentCultureIgnoreCase))
            {
                gameDataKeyList = new List<string>(gameData.Keys);
                return gameDataKeyList[i];
            }
        }
        return "";
    }
    
    public bool CheckPlayerIDMatchesUserName(string userID, string name)
    {
        if (gameData[userID].playerName == name)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
