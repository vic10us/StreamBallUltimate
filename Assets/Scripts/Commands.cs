#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwitchLib.Client.Models;
using TwitchLib.Unity;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;

public class Commands : MonoBehaviour
{
    //ON THIS DAY WE GATHER TO PUT AN END TO THE WAR THAT HAS WAGED ON SIMPATHEYS BLOOD PRESSURE
    //RAVONUS, PHYRODARKMATTER, BEGINBOT AND PUNCHYPENGUIN WAGED WAR ALL THE WAY TO THE 200% HYPE TRAIN
    //BUT IT DIDNT END THERE, THE BATTLE SPILLED OUT AND EFFECTED THE LIVES OF ALL BUG CLUB MEMBERS FOLLOWERS AND
    //LURKERS ALIKE. ON THIS DAY AND HOUR THE TREATY IS SIGNED BY ALL PARTIES SO THAT SIMPATHEY DOESNT DIE
    //BEGINBOT HAS SIGNED 
    //RAVONUS UNDECIDED 
    //PHYRODARKMATTER UNDECIDED 
    //PUNCHYPENGUIN HAS SIGNED
    //CODING CUBER HAS SIGNED 
    //MOTTZYMAKES 
    TwitchClient twitchClient;
    Client chatClient;
    JoinedChannel chatJoinedChannel;
    JoinedChannel botJoinedChannel;
    [SerializeField] GameData gameDataScript;
    [SerializeField] MarbleList marbleList;
    [SerializeField] GameController gameController;
    [SerializeField] JumpManager jumpManager;
    [SerializeField] Shop shop;
    const string help = "!join-join the game | !play-play the game when it is GAMETIME | play to earn money" +
        " |  save money to buy and equip new marbles";
    //const string playerAlreadyExists = " your user entry already exists, no need to join";
    const string noPlayerEntryExists = ", Please type '!join' to play";
    //const string playerEntryAdded = " you have joined the game";
    const string noMarbleWithNameExists = ", there is no marble with that name. Please type a valid marble name.";
    const string unlockedMarble1 = " has unlocked the ";
    const string unlockedMarble2 = " marble. Use '!equip ";
    const string unlockedMarble3 = "' to use your new marble.";
    const string unlockedMarble4 = " Current Balance: ";
    // const string notEnoughMoney = ", you do not have enough money to unlock ";
    const string skinAlreadyUnlocked1 = ", you already have the ";
    const string skinAlreadyUnlocked2 = " marble unlocked.";
    const string dontOwnThatSkin = ", you dont own that skin. Type !skins to see the skins you own.";
    const string notSubscribed1 = ", you can not use this command unless you give Simpagamebot permission to whisper your Twitch";
    const string notSubscribed2 = "Please type !acceptwhispers you can type !stopwhispers at any time to revoke whisper permissions.";
    const string marbleNotInShop = " marble is not in shop";
    const string cantGiveMoneyToPlayer1 = "Can not give money to ";
    const string cantGiveMoneyToPlayer2 = " because they are not entered in the game.";
    private Dictionary<Regex, CommandHandler> commandHandlers = new Dictionary<Regex, CommandHandler>();

    private void Start()
    {
        commandHandlers.Add(new Regex("^help$"), new CommandHandler { Handle = Help });
        commandHandlers.Add(new Regex("^join$"), new CommandHandler { Handle = Join });
        commandHandlers.Add(new Regex("^buy$"), new CommandHandler { Handle = Buy });
        commandHandlers.Add(new Regex("^equip$"), new CommandHandler { Handle = Equip });
        commandHandlers.Add(new Regex("^money$"), new CommandHandler { Handle = Money });
        commandHandlers.Add(new Regex("^inuse$"), new CommandHandler { Handle = InUse });
        commandHandlers.Add(new Regex("^skins$"), new CommandHandler { Handle = Skins });
        commandHandlers.Add(new Regex("^give$"), new CommandHandler { Handle = Give });
        commandHandlers.Add(new Regex("^price$"), new CommandHandler { Handle = Price });
        commandHandlers.Add(new Regex("^store$"), new CommandHandler { Handle = Store });
        commandHandlers.Add(new Regex("^setprice$"), new CommandHandler { Handle = SetPrice });
        commandHandlers.Add(new Regex("^play$"), new CommandHandler { Queue = false, Handle = Play });
        commandHandlers.Add(new Regex("^rotate$"), new CommandHandler { Queue = false, Handle = Rotate });
        //chatClient.SendMessage(chatJoinedChannel, help);
        //Setup();
    }

    public CommandHandler GetCommandHandler(string commandName) {
        var command = commandHandlers.FirstOrDefault(k => k.Key.IsMatch(commandName));
        if (command.Value == null) Debug.LogWarning($"Command handler not found for: {commandName}");
        return command.Value;
    }

    public void ExecuteCommand(Arrrgs a) {
        var command = GetCommandHandler(a.commandText);
        if (command == null) {
            Debug.LogWarning($"Command handler not found for: {a.commandText}");
            return;
        }
        command.Handle(a);
    }

    public void Setup()
    {
        twitchClient = FindObjectOfType<TwitchClient>();
        chatClient = twitchClient.client;
        chatJoinedChannel = twitchClient.joinedChannel;
        botJoinedChannel = twitchClient.botChannel;
    }

    //Help - provids a list of commands
    public void Help(Arrrgs e)
    {
        EnsureConnected();
        chatClient.SendMessage(chatJoinedChannel, help);
    }

    //Join - check if player data exists - if not create empty player data entry
    public void Join (Arrrgs e)
    {
        EnsureConnected();
        if (gameDataScript.CheckIfPlayerExists(e.userID))
        {
            chatClient.SendMessage(chatJoinedChannel, $"@{e.displayName} your user entry already exists, no need to join.");
            return;
        }
        gameDataScript.CreateNewPlayerEntry(e);
        chatClient.SendMessage(chatJoinedChannel, $"@{e.displayName} you have joined the game.");
    }

    //Money - checks if player data exists - if so returns how much money they have in chat
    public void Money(Arrrgs e)
    {
        if (chatClient == null) Setup();
        string playerID = e.userID;
        string userName = e.displayName;

        if (!gameDataScript.CheckIfPlayerExists(playerID))
        {
            chatClient.SendMessage(chatJoinedChannel, $"@{userName}{noPlayerEntryExists}");
            return;
        }

        chatClient.SendMessage(chatJoinedChannel, $"@{userName} money: ${gameDataScript.CheckPlayerMoney(playerID)}");
    }

    //Buy - checks if player data exists - if so checks if has enough money - if so then unlock skin
    public void Buy(Arrrgs e)
    {
        EnsureConnected();
        string playerID = e.userID; //Command.ChatMessage.UserId;
        string playerUserName = e.displayName; //Command.ChatMessage.Username;
        if (!gameDataScript.CheckIfPlayerExists(playerID))
        {
            chatClient.SendMessage(chatJoinedChannel, playerUserName + noPlayerEntryExists);
            return;
        }

        var commonName = e.commandArgs;
        /*
        for (int index = 0; index < e.Command.ArgumentsAsList.Count; index++)
        {
            commonName += e.Command.ArgumentsAsList[index].ToLower();
        }*/

        // Check if the marble with that name exists in the Marble List
        if (!marbleList.DoesMarbleCommonNameExist(commonName))
        {
            chatClient.SendMessage(chatJoinedChannel, playerUserName + noMarbleWithNameExists);
            return;
        }

        // Check if the marble with that name is currently in the Shop
        if (!shop.MarbleNamesInShop(commonName))
        {
            chatClient.SendMessage(chatJoinedChannel, commonName + marbleNotInShop);
            return;
        }

        int playerMoney = gameDataScript.CheckPlayerMoney(playerID);
        int marbleCost = marbleList.GetMarbleCostFromCommonName(commonName);
        if (gameDataScript.IsSkinUnlocked(playerID, commonName))
        {
            chatClient.SendMessage(chatJoinedChannel, playerUserName + skinAlreadyUnlocked1 + commonName + skinAlreadyUnlocked2);
            return;
        }

        if (playerMoney < marbleCost)
        {
            chatClient.SendMessage(chatJoinedChannel, $"{playerUserName}, you do not have enough money to unlock {commonName}. Balance: {playerMoney}");
            return;
        }

        gameDataScript.SubtractMoneyFromPlayerID(marbleCost, playerID);
        gameDataScript.UnlockSkinForPlayer(playerID, commonName);
        int currentMoney = gameDataScript.CheckPlayerMoney(playerID);
        chatClient.SendMessage(chatJoinedChannel, playerUserName + unlockedMarble1 +
            commonName + unlockedMarble2 + commonName + unlockedMarble3 + unlockedMarble4 + currentMoney);
    }


    //Equip - checks if player data exists - checks if they own that skin - equips the skin
    public void Equip(Arrrgs e)
    {
        EnsureConnected();
        var playerID = e.userID;
        var playerUserName = e.displayName;
        var commonName = e.commandArgs;

        // Check if the player has joined
        if (!gameDataScript.CheckIfPlayerExists(playerID))
        {
            chatClient.SendMessage(chatJoinedChannel, $"@{playerUserName}{noPlayerEntryExists}");
            return;
        }

        // Check if the marble exists
        if (!marbleList.DoesMarbleCommonNameExist(commonName))
        {
            chatClient.SendMessage(chatJoinedChannel, $"@{playerUserName}{noMarbleWithNameExists}");
            return;
        }

        // Check if the user owns that skin
        if (!gameDataScript.IsSkinUnlocked(playerID, commonName))
        {
            chatClient.SendMessage(chatJoinedChannel, playerUserName + dontOwnThatSkin);
            return;
        }

        gameDataScript.SetPlayerEquipSkin(playerID, commonName);
        chatClient.SendMessage(chatJoinedChannel, $"{playerUserName}, you now have the {commonName} skin in use.");
        Debug.Log(playerUserName + " equipt " + commonName);
    }

    //Equipted - checks if player data exists - checks what skin they have equipped - tells them what skin that is
    public void InUse(Arrrgs e)
    {
        EnsureConnected();
        string playerID = e.userID;
        string playerUserName = e.displayName;

        if (!gameDataScript.CheckIfPlayerExists(playerID))
        {
            chatClient.SendMessage(chatJoinedChannel, $"@{playerUserName}{noPlayerEntryExists}");
            return;
        }

        var commonName = gameDataScript.GetPlayerEquipSkin(playerID);
        chatClient.SendMessage(chatJoinedChannel, $"@{playerUserName}, is using the {commonName} skin!");
    }

    public void Play(Arrrgs e)
    {
        EnsureConnected();
        string userID = e.userID;
        string displayName = e.displayName;

        if (!gameDataScript.CheckIfPlayerExists(userID))
        {
            chatClient.SendMessage(chatJoinedChannel, $"@{displayName}{noPlayerEntryExists}");
            return;
        }

        if (gameController.currentState != GameState.GameTime)
        {
            chatClient.SendMessage(chatJoinedChannel, $"@{displayName}, sorry it's not gametime yet.");
            return;
        }

        switch (gameController.currentGameMode)
        {
            case GameMode.LongJump:
                jumpManager.CreateMarbleAndJump(e);
                break;
            case GameMode.HighJump:
                jumpManager.CreateMarbleAndHighJump(e);
                break;
            case GameMode.Race:
                break;
        }

    }

    //AcceptWhispers
    /*
    public void AcceptWhispers(OnChatCommandReceivedArgs e)
    {
        if (chatClient == null)
        {
            Setup();
            AttemptToAcceptWhispers(e);
        }
        else
        {
            AttemptToAcceptWhispers(e);
        }
    }
    */
    /*public void AttemptToAcceptWhispers(OnChatCommandReceivedArgs e)
    {
        Debug.Log("WHISPER,PLAYER ACTIVATED!");
        string userID = e.Command.ChatMessage.UserId;
        string playerName = e.Command.ChatMessage.Username;
        if (gameDataScript.CheckIfPlayerExists(userID))
        {
            Debug.Log("WHISPER,PLAYER DONE!");
            gameDataScript.SubscribePlayerToWhispers(userID);
            chatClient.SendWhisper(playerName, "THIS IS A TEST MESSEGE FROM SIMPA GAME BOT.");
        }
        else
        {
            chatClient.SendMessage(chatJoinedChannel, e.Command.ChatMessage.Username + noPlayerEntryExists);
        }
    }
    */

    public void Skins(Arrrgs e)
    {
        EnsureConnected();
        string userID = e.userID;
        string displayName = e.displayName;

        if (gameDataScript.CheckIfPlayerExists(userID))
        {
            string skinsPlayerOwns = gameDataScript.CheckSkins(e);
            // StartCoroutine(SkinsMessege(skinsPlayerOwns));
            chatClient.SendMessage(chatJoinedChannel, skinsPlayerOwns);
            //chatClient.SendMessage(chatJoinedChannel, "https://www.twitch.tv/simpagamebot");
        }
        else
        {
            chatClient.SendMessage(chatJoinedChannel, displayName + noPlayerEntryExists);
        }
    }

    public void AkaiEasterEgg(string name)
    {
        chatClient.SendMessage(chatJoinedChannel, $"{name} is hacking!");
    }

    public void Rotate(Arrrgs e) //Temporary command!!! TODO REMOVE
    {
        // if (e.userID == "73184979")
        if (e.isAdmin)
        {
            shop.ResetShop();
        }
    }

    public void Give(Arrrgs e)
    {
        EnsureConnected();
        string userID = e.userID;
        string displayName = e.displayName;

        if (!gameDataScript.CheckIfPlayerExists(userID)) {
            // user not found... tell them to join
            chatClient.SendMessage(chatJoinedChannel, $"@{displayName}{noPlayerEntryExists}");
            return;
        }

        if (e.multiCommand?.Count < 2) {
            // command is missin arguments show usage
            chatClient.SendMessage(chatJoinedChannel, $"@{displayName}, to give use !give [PlayerName] [Amount]");
            return;
        }

        string otherPlayerDisplayName = e.multiCommand[0].TrimStart('@');
        string PersonGettingMoney = gameDataScript.ConvertCommonNameToUserID(otherPlayerDisplayName);
        if (String.IsNullOrWhiteSpace(PersonGettingMoney))
        {
            // The player you are trying to send to doesn't exist...
            chatClient.SendMessage(chatJoinedChannel, $"Can not give money to {e.multiCommand[0]} because they are not entered in the game.");
            return;
        }

        if (!(gameDataScript.CheckIfPlayerExists(PersonGettingMoney) &&
        gameDataScript.CheckPlayerIDMatchesUserName(PersonGettingMoney, otherPlayerDisplayName))) {
            chatClient.SendMessage(chatJoinedChannel, $"Can not give money to {e.multiCommand[0]} because they are not entered in the game.");
            return;
        }

        int cost;
        if (!int.TryParse(e.multiCommand[1], out cost)) {
            chatClient.SendMessage(chatJoinedChannel, $"'{e.multiCommand[1]}' is not a valid money amount.");
            return;
        }

        if (e.isAdmin) {
            gameDataScript.AddMoneyToPlayerID(cost, PersonGettingMoney);
            chatClient.SendMessage(chatJoinedChannel, $"@{displayName} granted {e.multiCommand[0]} ${cost}");
            return;
        }

        int currentMoney = gameDataScript.CheckPlayerMoney(userID);
        if (currentMoney < cost)
        {
            chatClient.SendMessage(chatJoinedChannel, $"{displayName}, you can't give money you dont have.");
        }

        if (cost <= 0 || cost>10000)
        {
            chatClient.SendMessage(chatJoinedChannel, $"{displayName}, you can only give give between 1 and 10000");
        }

        gameDataScript.SubtractMoneyFromPlayerID(cost, userID);
        gameDataScript.AddMoneyToPlayerID(cost, PersonGettingMoney);
        chatClient.SendMessage(chatJoinedChannel, displayName+ " gave " + e.multiCommand[0] +" "+cost);
    }

    public void Price(Arrrgs e) {
        EnsureConnected();
        if (e.multiCommand.Count < 1) {
            chatClient.SendMessage(chatJoinedChannel, $"@{e.displayName}, you need to specify a marble name.");
            return;
        }

        var marble = marbleList.GetMarble(e.argumentsAsString);

        if (marble == null) {
            chatClient.SendMessage(chatJoinedChannel, $"@{e.displayName}, there is no marble with the name '{e.argumentsAsString}'.");
            return;
        }

        chatClient.SendMessage(chatJoinedChannel, $"@{e.displayName}, the price of {marble.name} is ${marble.cost}.");
    }

    public void Store(Arrrgs e) {
        EnsureConnected();
        var marbleList = shop.MarblesInShop();
        var marbleListString = marbleList.Select(m => $"{m.name}: ${m.cost}").Aggregate((o,n) => $"{o}, {n}");
        chatClient.SendMessage(chatJoinedChannel, $"@{e.displayName}, the shop currently has these marbles: [{marbleListString}]");
    }

    public void SetPrice(Arrrgs e) {
        EnsureConnected();
        // Silently ignore non-admins
        if (!e.isAdmin) { return; }
        // Missing arguments
        if (e.multiCommand.Count != 2) return;
        // Check that the price is a valid number between 0 and 9999
        if (!Int32.TryParse(e.multiCommand[1], out var price) || price < 0 || price > 9999) return;
        var shopMarble = shop.MarblesInShop().FirstOrDefault(m => m.commonName.Equals(e.multiCommand[0]));
        if (shopMarble != null) {
            shopMarble.cost = price;
            shop.ResetShop(false);
        }
        var marble = marbleList.GetMarble(e.multiCommand[0]);
        if (marble != null) {
            marble.cost = price;
        }
        // Check if the marble exists
        if (!marbleList.DoesMarbleCommonNameExist(e.multiCommand[0])) return;
        if (marbleList.SetMarbleCost(e.multiCommand[0], price) >= 0) {
            chatClient.SendMessage(chatJoinedChannel, $"Marble {e.multiCommand[0]} changed price! It is now ${price}");
        }
    }

    private void EnsureConnected() {
        if (chatClient == null) Setup();
    }
}
