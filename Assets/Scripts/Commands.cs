using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwitchLib.Client.Models;
using TwitchLib.Unity;
using System;
using System.Linq;
using System.Text.RegularExpressions;

public class CommandHandler {
    public bool Queue { get; set; } = true;
    public Action<Arrrgs> Handle;
}

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
    const string secretMsg = " is hacking";
    const string help = "!join-join the game | !play-play the game when it is GAMETIME | play to earn money" +
        " |  save money to buy and equip new marbles";
    const string playerAlreadyExists = " your user entry already exists, no need to join";
    const string noPlayerEntryExists = ", Please type '!join' to play";
    const string playerEntryAdded = " you have joined the game";
    const string noMarbleWithNameExists = ", there is no marble with that name. Please type a valid marble name.";
    const string unlockedMarble1 = " has unlocked the ";
    const string unlockedMarble2 = " marble. Use '!equip ";
    const string unlockedMarble3 = "' to use your new marble.";
    const string unlockedMarble4 = " Current Balance: ";
    const string notEnoughMoney = ", you do not have enough money to unlock ";
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
        if (chatClient == null)
        {
            Setup();
            //chatClient.SendMessage(chatJoinedChannel, help);
            AttemptToHelp(e);
        }
        else
        {
            AttemptToHelp(e);
        }
    }

    private void AttemptToHelp(Arrrgs e)
    {
        chatClient.SendMessage(chatJoinedChannel, help);
    }

    //Join - check if player data exists - if not create empty player data entry
    public void Join (Arrrgs e)
    {
        if (chatClient == null)
        {
            Setup();
            AttemptToJoin(e);
        }
        else
        {
            AttemptToJoin(e);
        }
    }

    private void AttemptToJoin(Arrrgs e)
    {
        if (gameDataScript.CheckIfPlayerExists(e.userID))
        {
            chatClient.SendMessage(chatJoinedChannel, e.displayName + playerAlreadyExists);
        }
        else
        {
            gameDataScript.CreateNewPlayerEntry(e);
            chatClient.SendMessage(chatJoinedChannel, e.displayName + playerEntryAdded);
        }
    }

    //Money - checks if player data exists - if so returns how much money they have in chat
    public void Money(Arrrgs e)
    {
        if (chatClient == null)
        {
            Setup();
            AttemptToCheckMoney(e);
        }
        else
        {
            AttemptToCheckMoney(e);
        }
    }
    private void AttemptToCheckMoney(Arrrgs e)
    {
        string playerID = e.userID;
        string userName = e.displayName;

        if (gameDataScript.CheckIfPlayerExists(playerID))
        {
            chatClient.SendMessage(chatJoinedChannel, userName + " money: " + gameDataScript.CheckPlayerMoney(playerID));
        }
        else
        {
            chatClient.SendMessage(chatJoinedChannel, userName + noPlayerEntryExists);
        }
    }

    //Buy - checks if player data exists - if so checks if has enough money - if so then unlock skin
    public void Buy(Arrrgs e)
    {
        if (chatClient == null)
        {
            Setup();
        }
        AttemptToBuy(e);
    }
    
    public void AttemptToBuy(Arrrgs e)
    {
        string commonName = "";
        string playerID = e.userID; //Command.ChatMessage.UserId;
        string playerUserName = e.displayName; //Command.ChatMessage.Username;
        if (gameDataScript.CheckIfPlayerExists(playerID))
        {
            commonName = e.commandArgs;
            /*
            for (int index = 0; index < e.Command.ArgumentsAsList.Count; index++)
            {
                commonName += e.Command.ArgumentsAsList[index].ToLower();
            }*/
            Debug.Log(commonName);
            // Check if the marble with that name exists in the Marble List
            if (marbleList.DoesMarbleCommonNameExist(commonName))
            {
                // Check if the marble with that name is currently in the Shop
                if (shop.MarbleNamesInShop(commonName))
                {
                    int playerMoney = gameDataScript.CheckPlayerMoney(playerID);
                    int marbleCost = marbleList.GetMarbleCostFromCommonName(commonName);
                    if (gameDataScript.IsSkinUnlocked(playerID, commonName))
                    {
                        chatClient.SendMessage(chatJoinedChannel, playerUserName + skinAlreadyUnlocked1 + commonName + skinAlreadyUnlocked2);
                    }
                    else
                    {
                        if (playerMoney >= marbleCost)
                        {
                            gameDataScript.SubtractMoneyFromPlayerID(marbleCost, playerID);
                            gameDataScript.UnlockSkinForPlayer(playerID, commonName);
                            int currentMoney = gameDataScript.CheckPlayerMoney(playerID);
                            chatClient.SendMessage(chatJoinedChannel, playerUserName + unlockedMarble1 +
                                commonName + unlockedMarble2 + commonName + unlockedMarble3 + unlockedMarble4 + currentMoney);
                        }
                        else
                        {
                            chatClient.SendMessage(chatJoinedChannel, playerUserName + notEnoughMoney + commonName);
                        }
                    }
                }
                else
                {
                    chatClient.SendMessage(chatJoinedChannel, commonName + marbleNotInShop);
                }
            }
            else
            {
                chatClient.SendMessage(chatJoinedChannel, playerUserName + noMarbleWithNameExists);
            }
        }
        else
        {
            chatClient.SendMessage(chatJoinedChannel, playerUserName + noPlayerEntryExists);
        }
    }


    //Equip - checks if player data exists - checks if they own that skin - equips the skin
    public void Equip(Arrrgs e)
    {
        if (chatClient == null)
        {
            Setup();
        }
        AttemptToEquip(e);
    }

    private void AttemptToEquip(Arrrgs e)
    {
        string commonName = "";
        string playerID = e.userID;
        string playerUserName = e.displayName;
        commonName = e.commandArgs;
        /*
        for (int index = 0; index < e.Command.ArgumentsAsList.Count; index++)
        {
            commonName += e.Command.ArgumentsAsList[index].ToLower();
        }*/

        if (gameDataScript.CheckIfPlayerExists(playerID))
        {
            if (marbleList.DoesMarbleCommonNameExist(commonName))
            {
                if (gameDataScript.IsSkinUnlocked(playerID, commonName))
                {
                    gameDataScript.SetPlayerEquipSkin(playerID, commonName);
                    chatClient.SendMessage(chatJoinedChannel, $"{playerUserName}, you now have the {commonName} skin in use.");
                    Debug.Log(playerUserName+" equipt "+commonName );
                }
                else
                {
                    chatClient.SendMessage(chatJoinedChannel, playerUserName + dontOwnThatSkin);
                }
            }
            else
            {
                chatClient.SendMessage(chatJoinedChannel, playerUserName + noMarbleWithNameExists);
            }
        }
        else
        {
            chatClient.SendMessage(chatJoinedChannel, playerUserName + noPlayerEntryExists);
        }
    }

    //Equipted - checks if player data exists - checks what skin they have equipped - tells them what skin that is
    public void InUse(Arrrgs e)
    {
        if (chatClient == null)
        {
            Setup();
        }
        AttemptToInUse(e);
    }

    private void AttemptToInUse(Arrrgs e)
    {
        string playerID = e.userID;
        string playerUserName = e.displayName;

        if (gameDataScript.CheckIfPlayerExists(playerID))
        {
            var commonName = gameDataScript.GetPlayerEquipSkin(playerID);
            chatClient.SendMessage(chatJoinedChannel, playerUserName + " is using the " + commonName + " skin!");
        }
        else
        {
            chatClient.SendMessage(chatJoinedChannel, playerUserName + noPlayerEntryExists);
        }

    }

    public void Play(Arrrgs e)
    {
        if (chatClient == null)
        {
            Setup();
            AttemptToPlay(e);
        }
        else
        {
            AttemptToPlay(e);
        }
    }

    private void AttemptToPlay(Arrrgs e)
    {
        string userID = e.userID;
        string displayName = e.displayName;

        if (gameDataScript.CheckIfPlayerExists(userID))
        {
            if (gameController.currentState == GameState.GameTime)
            {
                if (gameController.currentGameMode == GameMode.LongJump)
                {
                    jumpManager.CreateMarbleAndJump(e);
                }
                else if (gameController.currentGameMode == GameMode.HighJump)
                {
                    jumpManager.CreateMarbleAndHighJump(e);
                }
                else if (gameController.currentGameMode == GameMode.Race)
                {

                }
            }
            else
            {
                return;
            }
        }
        else
        {
            chatClient.SendMessage(chatJoinedChannel, displayName + noPlayerEntryExists);
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
        if (chatClient == null)
        {
            Setup();
        }
        AttemptToSkins(e);
    }

    public void AttemptToSkins(Arrrgs e)
    {
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
    IEnumerator SkinsMessege(string skinsList)
    {
        yield return new WaitForSeconds(7);
        chatClient.SendMessage(botJoinedChannel, skinsList);
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
            FindObjectOfType<Shop>().ResetShop();
        }
    }

    public void Give(Arrrgs e)
    {
        if (chatClient == null)
        {
            Setup();
            AttemptToGive(e);
        }
        else
        {
            AttemptToGive(e);
        }
    }

    public void AttemptToGive(Arrrgs e)
    {
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
}
