#pragma warning disable 649
#pragma warning disable IDE0051 // Remove unused private members
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable IteratorNeverReturns

using System.Collections.Generic;
using UnityEngine;
using TwitchLib.Client.Models;
using TwitchLib.Unity;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using StreamBallUltimate.Assets.Extensions;

public class Commands : MonoBehaviour
{
    //ON THIS DAY WE GATHER TO PUT AN END TO THE WAR THAT HAS WAGED ON SIMPATHEY'S BLOOD PRESSURE
    //RAVONUS, PHYRODARKMATTER, BEGINBOT AND PUNCHYPENGUIN WAGED WAR ALL THE WAY TO THE 200% HYPE TRAIN
    //BUT IT DIDN'T END THERE, THE BATTLE SPILLED OUT AND EFFECTED THE LIVES OF ALL BUG CLUB MEMBERS FOLLOWERS AND
    //LURKERS ALIKE. ON THIS DAY AND HOUR THE TREATY IS SIGNED BY ALL PARTIES SO THAT SIMPATHEY DOESN'T DIE
    //BEGINBOT HAS SIGNED 
    //RAVONUS UNDECIDED 
    //PHYRODARKMATTER UNDECIDED 
    //PUNCHYPENGUIN HAS SIGNED
    //CODING CUBER HAS SIGNED 
    //MOTTZYMAKES 
    private TwitchClient twitchClient;
    private Client chatClient;
    private JoinedChannel chatJoinedChannel;
    [SerializeField] public GameData gameDataScript;
    [SerializeField] public MarbleList marbleList;
    [SerializeField] public GameController gameController;
    [SerializeField] public JumpManager jumpManager;
    [SerializeField] public Shop shop;
    private const string noPlayerEntryExists = ", Please type '!join' to play";
    private readonly Dictionary<Regex, CommandHandler> _commandHandlers = new Dictionary<Regex, CommandHandler>();
    private readonly Dictionary<string, string> _commandHelp = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

    private void SetupCommands() {
        var commandMethods = this.GetType().GetMethods().Where(m => m.GetCustomAttributes(typeof(CommandAttribute), false).Any());
        var commandResults = commandMethods.ToList().Select(m => new { 
            Info = m,
            CommandInfo = (CommandAttribute)m.GetCustomAttributes(typeof(CommandAttribute), false).First(),
            HelpInfo = (HelpInfoAttribute)m.GetCustomAttributes(typeof(HelpInfoAttribute), false).FirstOrDefault()
        }).ToList();
        foreach (var x in commandResults.Where(cr => cr.CommandInfo.Enabled)) {
            _commandHandlers.Add(new Regex(x.CommandInfo.MatchExpression, RegexOptions.IgnoreCase),
            new CommandHandler {
                Handle = a =>{
                    x.Info.Invoke(this, new object[] {a});
                },
                Queue = x.CommandInfo.Queue
            });
            _commandHelp.Add(x.CommandInfo.Name, $"{(x.CommandInfo.AdminOnly ? "[ADMIN ONLY] " : "")}{x.HelpInfo.HelpText}");
        }
    }

    // ReSharper disable once UnusedMember.Local
    private void Start()
    {
        SetupCommands();
    }

    public CommandHandler GetCommandHandler(string commandName) {
        var command = _commandHandlers.FirstOrDefault(k => k.Key.IsMatch(commandName));
        if (command.Value == null) Debug.LogWarning($"Command handler not found for: {commandName}");
        return command.Value;
    }

    public void ExecuteCommand(Arrrgs a) {
        var command = GetCommandHandler(a.CommandText);
        if (command == null) {
            Debug.LogWarning($"Command handler not found for: {a.CommandText}");
            return;
        }
        command.Handle(a);
    }

    public void Setup()
    {
        twitchClient = FindObjectOfType<TwitchClient>();
        chatClient = twitchClient.client;
        chatJoinedChannel = twitchClient.joinedChannel;
    }
    
    // [HelpInfo(HelpText = "Use `!bob` to find out who is your uncle!")]
    // [Command(Name = "bob", MatchExpression = "^bob$", Enabled = true)]
    // public void Bob(Arrrgs e)
    // {
    //     StartCoroutine(chatClient.SendMessages(chatJoinedChannel, "Bob's your uncle!"));
    // }

    //Help - provides a list of commands
    [HelpInfo(HelpText = "Use `!help [command]` for help with a command. or `!help` alone for a list of commands.")]
    [Command(Name = "help", MatchExpression = "^help$")]
    public void Help(Arrrgs e)
    {
        EnsureConnected();
        if (e.MultiCommand.Any() && _commandHelp.ContainsKey(e.MultiCommand.First())) {
            var helpText = _commandHelp[e.MultiCommand.First()];
            StartCoroutine(chatClient.SendMessages(chatJoinedChannel, helpText));
            return;
        }

        var commands = _commandHelp.Select(c => $"!{c.Key}");
        var helpMessage = $"Here are all the commands: [{commands.Aggregate((o,n) => $"{o}, {n}")}] Type !help [command] for more";

        StartCoroutine(chatClient.SendMessages(chatJoinedChannel, helpMessage));
    }

    //Join - check if player data exists - if not create empty player data entry
    [HelpInfo(HelpText = "Use `!join` to add yourself to the game")]
    [Command(Name = "join", MatchExpression = "^join$")]
    public void Join (Arrrgs e)
    {
        EnsureConnected();
        if (gameDataScript.CheckIfPlayerExists(e.UserID))
        {
            chatClient.SendMessage(chatJoinedChannel, $"@{e.DisplayName} your user entry already exists, no need to join again.");
            return;
        }
        gameDataScript.CreateNewPlayerEntry(e);
        chatClient.SendMessage(chatJoinedChannel, $"@{e.DisplayName} you have joined the game. Enjoy!");
    }

    [HelpInfo(HelpText = "Use '!money' to query how much money you have in the bank.")]
    [Command(Name = "money", MatchExpression = "^money$")]
    //Money - checks if player data exists - if so returns how much money they have in chat
    public void Money(Arrrgs e)
    {
        if (chatClient == null) Setup();
        var playerID = e.UserID;
        var userName = e.DisplayName;

        if (!gameDataScript.CheckIfPlayerExists(playerID))
        {
            chatClient.SendMessage(chatJoinedChannel, $"@{userName}{noPlayerEntryExists}");
            return;
        }

        chatClient.SendMessage(chatJoinedChannel, $"@{userName}, you have ${gameDataScript.CheckPlayerMoney(playerID)} in the bank.");
    }

    [HelpInfo(HelpText = "Use `!sell` to sell a marble you have previously purchased for 80% of its current value")]
    [Command(Name = "sell", MatchExpression = "^sell$", Enabled = true)]
    public void Bob(Arrrgs e)
    {
        EnsureConnected();
        var playerID = e.UserID;
        var playerUserName = e.DisplayName;

        if (!gameDataScript.CheckIfPlayerExists(playerID))
        {
            chatClient.SendMessage(chatJoinedChannel, $"@{playerUserName}{noPlayerEntryExists}");
            return;
        }

        var commonName = e.CommandArgs;

        if (commonName.Equals("default", StringComparison.InvariantCultureIgnoreCase))
        {
            chatClient.SendMessage(chatJoinedChannel, $"@{playerUserName}, you can't sell the default marble.");
            return;
        }

        // Check if the marble with that name exists in the Marble List
        if (!marbleList.DoesMarbleCommonNameExist(commonName))
        {
            chatClient.SendMessage(chatJoinedChannel, $"@{playerUserName}, there is no marble with that name. Please type a valid marble name.");
            return;
        }
        
        var playerMoney = gameDataScript.CheckPlayerMoney(playerID);
        var marbleCost = marbleList.GetMarbleCostFromCommonName(commonName);
        if (!gameDataScript.IsSkinUnlocked(playerID, commonName))
        {
            chatClient.SendMessage(chatJoinedChannel, $"@{playerUserName}, you don't own the {commonName} marble.");
            return;
        }

        var saleValue = (int)(marbleCost * 0.8f);
        if (saleValue <= 0)
        {
            chatClient.SendMessage(chatJoinedChannel, $"@{playerUserName}, you can't sell the {commonName} marble because it is worthless.");
            return;
        }

        gameDataScript.AddMoneyToPlayerID(saleValue, playerID);
        var newSkin = gameDataScript.RemoveSkinFromPlayer(playerID, commonName);
        var currentMoney = gameDataScript.CheckPlayerMoney(playerID);
        var message = $"@{playerUserName} you sold your {commonName} marble for ${saleValue}. You are now using the {newSkin} marble. Current Balance: ${currentMoney}";
        StartCoroutine(chatClient.SendMessages(chatJoinedChannel, message));
    }

    [HelpInfo(HelpText = "Use '!buy [marble_name]' to buy a marble.")]
    [Command(Name = "buy", MatchExpression = "^buy$")]
    //Buy - checks if player data exists - if so checks if has enough money - if so then unlock skin
    public void Buy(Arrrgs e)
    {
        EnsureConnected();
        var playerID = e.UserID; //Command.ChatMessage.UserId;
        var playerUserName = e.DisplayName; //Command.ChatMessage.Username;
        if (!gameDataScript.CheckIfPlayerExists(playerID))
        {
            chatClient.SendMessage(chatJoinedChannel, $"@{playerUserName}{noPlayerEntryExists}");
            return;
        }

        var commonName = e.CommandArgs;

        // Check if the marble with that name exists in the Marble List
        if (!marbleList.DoesMarbleCommonNameExist(commonName))
        {
            chatClient.SendMessage(chatJoinedChannel, $"@{playerUserName}, there is no marble with that name. Please type a valid marble name.");
            return;
        }

        // Check if the marble with that name is currently in the Shop
        if (!shop.MarbleNamesInShop(commonName))
        {
            chatClient.SendMessage(chatJoinedChannel, $"@{playerUserName}, the {commonName} marble is not in the shop.");
            return;
        }

        var playerMoney = gameDataScript.CheckPlayerMoney(playerID);
        var marble = marbleList.GetMarble(commonName);
        if (gameDataScript.IsSkinUnlocked(playerID, commonName))
        {
            chatClient.SendMessage(chatJoinedChannel, $"@{playerUserName}, you already have the {marble.commonName} marble unlocked.");
            return;
        }

        if (playerMoney < marble.cost)
        {
            chatClient.SendMessage(chatJoinedChannel, $"{playerUserName}, you do not have enough money to unlock {marble.commonName}. Balance: {playerMoney}");
            return;
        }

        gameDataScript.SubtractMoneyFromPlayerID(marble.cost, playerID);
        gameDataScript.UnlockSkinForPlayer(playerID, marble.commonName);
        var currentMoney = gameDataScript.CheckPlayerMoney(playerID);
        var message =$"@{playerUserName} has unlocked the {marble.commonName} marble. Use '!equip {marble.commonName}' to use your new marble. Current Balance: ${currentMoney}";
        StartCoroutine(chatClient.SendMessages(chatJoinedChannel, message));
    }


    //Equip - checks if player data exists - checks if they own that skin - equips the skin
    [HelpInfo(HelpText = "Use '!equip [marble_name]' to equip a marble you have in your inventory.")]
    [Command(Name = "equip", MatchExpression = "^equip$")]
    public void Equip(Arrrgs e)
    {
        EnsureConnected();
        var playerID = e.UserID;
        var playerUserName = e.DisplayName;
        var commonName = e.CommandArgs;

        // Check if the player has joined
        if (!gameDataScript.CheckIfPlayerExists(playerID))
        {
            chatClient.SendMessage(chatJoinedChannel, $"@{playerUserName}{noPlayerEntryExists}");
            return;
        }

        // Check if the marble exists
        var marble = marbleList.GetMarble(commonName);
        if (marble == null)
        {
            chatClient.SendMessage(chatJoinedChannel, $"@{playerUserName}, there is no marble with that name. Please type a valid marble name.");
            return;
        }

        // Check if the user owns that skin
        if (!gameDataScript.IsSkinUnlocked(playerID, marble.commonName))
        {
            chatClient.SendMessage(chatJoinedChannel, $"@{playerUserName}, you don't own that skin. Type !skins to see the skins you own.");
            return;
        }

        // Check if the user already has that skin selected.
        var selectedSkin = gameDataScript.GetPlayerEquipSkin(playerID);
        if (selectedSkin.Equals(marble.commonName, StringComparison.InvariantCultureIgnoreCase))
        {
            chatClient.SendMessage(chatJoinedChannel, $"@{playerUserName}, {selectedSkin} is already your current skin.");
            return;
        }

        gameDataScript.SetPlayerEquipSkin(playerID, marble.commonName);
        chatClient.SendMessage(chatJoinedChannel, $"{playerUserName}, you now have the {marble.commonName} skin in use.");
    }

    //Equipted - checks if player data exists - checks what skin they have equipped - tells them what skin that is
    [HelpInfo(HelpText = "Use '!inuse' see what marble is currently equipped.")]
    [Command(Name = "inuse", MatchExpression = "^inuse$")]
    public void InUse(Arrrgs e)
    {
        EnsureConnected();
        var playerID = e.UserID;
        var playerUserName = e.DisplayName;

        if (!gameDataScript.CheckIfPlayerExists(playerID))
        {
            chatClient.SendMessage(chatJoinedChannel, $"@{playerUserName}{noPlayerEntryExists}");
            return;
        }

        var commonName = gameDataScript.GetPlayerEquipSkin(playerID);
        chatClient.SendMessage(chatJoinedChannel, $"@{playerUserName}, is using the {commonName} skin!");
    }


    [HelpInfo(HelpText = "Use '!gametime' Start the game now!")]
    [Command(Name = "gametime", MatchExpression = "^gametime", Queue = false, AdminOnly = true)]
    public void GameTime(Arrrgs e)
    {
        EnsureConnected();
        if (!e.IsAdmin) return;
        // if (gameController.currentState != GameState.GameTime) return;
        gameController.PlayNow();
    }

    [HelpInfo(HelpText = "Use '!stopgame' Stop the game now!")]
    [Command(Name = "stopgame", MatchExpression = "^stopgame", Queue = false, AdminOnly = true)]
    public void StopGame(Arrrgs e)
    {
        EnsureConnected();
        if (!e.IsAdmin) return;
        // if (gameController.currentState == GameState.GameTime) return;
        gameController.DowntimeNow();
    }


    [HelpInfo(HelpText = "Use '!play' to play the game! First time is free. $50 to replay")]
    [Command(Name = "play", MatchExpression = "^play$", Queue = false)]
    public void Play(Arrrgs e)
    {
        EnsureConnected();
        var userID = e.UserID;
        var displayName = e.DisplayName;

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

    [HelpInfo(HelpText = "Use '!skins' to get all the skins that you have in your inventory.")]
    [Command(Name = "skins", MatchExpression = "^skins$")]
    public void Skins(Arrrgs e)
    {
        EnsureConnected();
        var userID = e.UserID;
        var displayName = e.DisplayName;

        if (!gameDataScript.CheckIfPlayerExists(userID))
        {
            chatClient.SendMessage(chatJoinedChannel, $"@{displayName}{noPlayerEntryExists}");
            return;
        }
        var skinsPlayerOwns = gameDataScript.CheckSkins(e);
        chatClient.SendMessage(chatJoinedChannel, skinsPlayerOwns);
    }

    public void AkaiEasterEgg(string playerName)
    {
        chatClient.SendMessage(chatJoinedChannel, $"{playerName} is hacking!");
    }

    [HelpInfo(HelpText = "Use '!rotate' to randomize the shop items.")]
    [Command(Name = "rotate", MatchExpression = "^rotate$", Queue = false, AdminOnly = true)]
    public void Rotate(Arrrgs e)
    {
        if (e.IsAdmin)
        {
            shop.ResetShop();
        }
    }

    [HelpInfo(HelpText = "Use '!give @[player_name] [amount]' to transfer money from your bank account to another player.")]
    [Command(Name = "give", MatchExpression = "^give$")]
    public void Give(Arrrgs e)
    {
        EnsureConnected();
        var userID = e.UserID;
        var displayName = e.DisplayName;

        if (!gameDataScript.CheckIfPlayerExists(userID)) {
            // user not found... tell them to join
            chatClient.SendMessage(chatJoinedChannel, $"@{displayName}{noPlayerEntryExists}");
            return;
        }

        if (e.MultiCommand == null || e.MultiCommand.Count < 2) {
            // command is missing arguments show usage
            chatClient.SendMessage(chatJoinedChannel, $"@{displayName}, to give use !give [PlayerName] [Amount]");
            return;
        }

        var otherPlayerDisplayName = e.MultiCommand?[0].TrimStart('@');
        var PersonGettingMoney = gameDataScript.ConvertCommonNameToUserID(otherPlayerDisplayName);

        if (string.IsNullOrWhiteSpace(PersonGettingMoney) || !(gameDataScript.CheckIfPlayerExists(PersonGettingMoney) &&
        gameDataScript.CheckPlayerIDMatchesUserName(PersonGettingMoney, otherPlayerDisplayName))) {
            // The player you are trying to send to doesn't exist...
            chatClient.SendMessage(chatJoinedChannel, $"@{displayName}, you can not give money to {e.MultiCommand[0]} because they are not entered in the game.");
            return;
        }

        if (!int.TryParse(e.MultiCommand[1], out var cost)) {
            chatClient.SendMessage(chatJoinedChannel, $"'{e.MultiCommand?[1]}' is not a valid money amount.");
            return;
        }

        if (e.IsAdmin) {
            gameDataScript.AddMoneyToPlayerID(cost, PersonGettingMoney);
            chatClient.SendMessage(chatJoinedChannel, $"@{displayName} granted {e.MultiCommand[0]} ${cost}");
            return;
        }

        var currentMoney = gameDataScript.CheckPlayerMoney(userID);
        if (currentMoney < cost)
        {
            chatClient.SendMessage(chatJoinedChannel, $"@{displayName}, you can't give money you don't have.");
        }

        if (cost <= 0 || cost>10000)
        {
            chatClient.SendMessage(chatJoinedChannel, $"@{displayName}, you can only give give between 1 and 10000");
        }

        gameDataScript.SubtractMoneyFromPlayerID(cost, userID);
        gameDataScript.AddMoneyToPlayerID(cost, PersonGettingMoney);
        chatClient.SendMessage(chatJoinedChannel, $"@{displayName} gave {e.MultiCommand[0]} ${cost}");
    }

    [HelpInfo(HelpText = "Use '!price [marble_name]' to get the price of any marble in the game.")]
    [Command(Name = "price", MatchExpression = "^price$")]
    public void Price(Arrrgs e) {
        EnsureConnected();
        if (e.MultiCommand.Count < 1) {
            chatClient.SendMessage(chatJoinedChannel, $"@{e.DisplayName}, you need to specify a marble name.");
            return;
        }

        var marble = marbleList.GetMarble(e.ArgumentsAsString);

        if (marble == null) {
            chatClient.SendMessage(chatJoinedChannel, $"@{e.DisplayName}, there is no marble with the name '{e.ArgumentsAsString}'.");
            return;
        }

        chatClient.SendMessage(chatJoinedChannel, $"@{e.DisplayName}, the price of {marble.name} is ${marble.cost}.");
    }

    [HelpInfo(HelpText = "Use '!store' to get the current items in the store.")]
    [Command(Name = "store", MatchExpression = "^store$")]
    public void Store(Arrrgs e) {
        EnsureConnected();
        var marbles = shop.MarblesInShop();
        var marbleListString = marbles.Select(m => $"{m.name}: ${m.cost}").Aggregate((o,n) => $"{o}, {n}");
        chatClient.SendMessage(chatJoinedChannel, $"@{e.DisplayName}, the shop currently has these marbles: [{marbleListString}]");
    }

    [HelpInfo(HelpText = "Use '!rename [marble_name] [new_name]' to change the name of a marble in the inventory.")]
    [Command(Name = "rename", MatchExpression = "^rename", Queue = false, AdminOnly = true)]
    public void RenameMarble(Arrrgs e)
    {
        EnsureConnected();
        // Silently ignore non-admins
        if (!e.IsAdmin) { return; }
        // Missing arguments
        if (e.MultiCommand.Count != 2) return;
        if (!marbleList.IsValidName(e.MultiCommand[1]))
        {
            chatClient.SendMessage(chatJoinedChannel, $"You cannot rename the marble {e.MultiCommand[0]} to {e.MultiCommand[1]} because {e.MultiCommand[1]} not a valid name.");
            return;
        }
        var existingMarble = marbleList.GetMarble(e.MultiCommand[1]);
        if (existingMarble != null)
        {
            chatClient.SendMessage(chatJoinedChannel, $"You cannot rename the marble {e.MultiCommand[0]} to {e.MultiCommand[1]} because {e.MultiCommand[1]} already exists.");
            return;
        }
        var shopMarble = shop.MarblesInShop().FirstOrDefault(m => m.commonName.Equals(e.MultiCommand[0]));
        if (shopMarble != null)
        {
            shopMarble.commonName = e.MultiCommand[1];
            shop.ResetShop(false);
        }
        var marble = marbleList.GetMarble(e.MultiCommand[0]);
        if (marble != null)
        {
            marble.commonName = e.MultiCommand[1];
        }
        if (!marbleList.DoesMarbleCommonNameExist(e.MultiCommand[1])) return;
        if (marbleList.SetMarbleName(e.MultiCommand[0], e.MultiCommand[1]) >= 0)
        {
            chatClient.SendMessage(chatJoinedChannel, $"Marble {e.MultiCommand[0]} changed name! It is now called {e.MultiCommand[1]}");
        }

        gameDataScript.RenameAllPlayerSkins(e.MultiCommand[0], e.MultiCommand[1]);
    }

    [HelpInfo(HelpText = "Use '!setprice [marble_name] [amount]' to change the price a marble in the inventory.")]
    [Command(Name = "setprice", MatchExpression = "^setprice$", Queue = false, AdminOnly = true)]
    public void SetPrice(Arrrgs e) {
        EnsureConnected();
        // Silently ignore non-admins
        if (!e.IsAdmin) { return; }
        // Missing arguments
        if (e.MultiCommand.Count != 2) return;
        // Check that the price is a valid number between 0 and 9999
        if (!int.TryParse(e.MultiCommand[1], out var price) || price < 0 || price > 9999) return;
        var shopMarble = shop.MarblesInShop().FirstOrDefault(m => m.commonName.Equals(e.MultiCommand[0]));
        if (shopMarble != null) {
            shopMarble.cost = price;
            shop.ResetShop(false);
        }
        var marble = marbleList.GetMarble(e.MultiCommand[0]);
        if (marble != null) {
            marble.cost = price;
        }
        // Check if the marble exists
        if (!marbleList.DoesMarbleCommonNameExist(e.MultiCommand[0])) return;
        if (marbleList.SetMarbleCost(e.MultiCommand[0], price) >= 0) {
            chatClient.SendMessage(chatJoinedChannel, $"Marble {e.MultiCommand[0]} changed price! It is now ${price}");
        }
    }

    private void EnsureConnected() {
        if (chatClient == null) Setup();
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
            chatClient.SendWhisper(playerName, "THIS IS A TEST MESSAGE FROM SIMPA GAME BOT.");
        }
        else
        {
            chatClient.SendMessage(chatJoinedChannel, e.Command.ChatMessage.Username + noPlayerEntryExists);
        }
    }
    */
}
