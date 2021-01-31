#pragma warning disable 649
#pragma warning disable IDE0051 // Remove unused private members
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable IteratorNeverReturns

using UnityEngine;
using TwitchLib.Client.Models;
using TwitchLib.Unity;
using TwitchLib.Client.Events;
using System;
using System.Linq;
using UnityEngine.UI;

public class TwitchClient : MonoBehaviour
{
    [SerializeField] public Text debug;

    // Client Object is defined in Twitch Lib
    public Client client;
    public JoinedChannel joinedChannel;
    public JoinedChannel botChannel;
    private CommandQueue commandQueue;
    private PubSub pubSub;
    private string channel_name;
    private string bot_name;

    private void Awake()
    {
        GlobalConfiguration.LoadConfiguration();
    }

    void Start()
    {
        commandQueue = FindObjectOfType<CommandQueue>();
        //This script should always run in background if game application is running
        Application.runInBackground = true;

        //set up bot and tell what channel to join
        bot_name = GlobalConfiguration.bot_name;
        channel_name = GlobalConfiguration.channel_name;
        var credentials = new ConnectionCredentials(GlobalConfiguration.bot_name, GlobalConfiguration.bot_access_token);

        client = new Client();
        client.Initialize(credentials, channel_name);

        //connect bot to channel
        client.Connect();
        client.OnJoinedChannel += ClientOnJoinedChannel;
        client.OnChatCommandReceived += MyCommandReceivedFunction;
        client.OnWhisperSent += Client_OnWhisperSent;
        client.OnWhisperReceived += Client_OnWhisperReceived;
    }
    
    private void Client_OnWhisperSent(object sender, OnWhisperSentArgs e)
    {
        Debug.Log(sender.ToString());
        Debug.Log(e.Receiver);
    }

    private void ClientOnJoinedChannel(object sender, OnJoinedChannelArgs e)
    {
        joinedChannel = new JoinedChannel(channel_name);
        botChannel = new JoinedChannel(bot_name);
    }

    private void MyCommandReceivedFunction(object sender, OnChatCommandReceivedArgs e)
    {
        var chatArgs = new Arrrgs
        {
            MessageType = MessageType.Command,
            IsMod = e.Command.ChatMessage.IsModerator,
            IsBroadcaster = e.Command.ChatMessage.IsBroadcaster,
            Message = e.Command.ChatMessage.Message,
            UserID = e.Command.ChatMessage.UserId,
            DisplayName = e.Command.ChatMessage.DisplayName,
            CommandText = e.Command.CommandText,
            MultiCommand = e.Command.ArgumentsAsList,
            ArgumentsAsString = e.Command.ArgumentsAsString,
        };

        if (e.Command.ArgumentsAsList.Any())
            chatArgs.CommandArgs = e.Command.ArgumentsAsList?.Aggregate((o, n) => $"{o}{n}");

        commandQueue.FirstCommandBuckets(chatArgs);
        debug.text = (e.Command.ChatMessage.Username);
    }

    private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
    {
        var chatArgs = new Arrrgs
        {
            MessageType = MessageType.Whisper,
            Message = e.WhisperMessage.Message,
            UserID = e.WhisperMessage.UserId,
            DisplayName = e.WhisperMessage.DisplayName,
            CommandText = ConvertWhisperToCommand(e.WhisperMessage.Message),
            CommandArgs = ConvertWhisperToArguments(e.WhisperMessage.Message)
        };
        commandQueue.FirstCommandBuckets(chatArgs);
    }

    private static string ConvertWhisperToCommand(string whisper)
    {
        if (string.IsNullOrEmpty(whisper)) return whisper;
        whisper = whisper.Substring(1);
        var commandArray = whisper.Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        return (commandArray[0].ToLower());
    }
    
    private static string ConvertWhisperToArguments(string whisper)
    {
        if (string.IsNullOrEmpty(whisper))
        {
            return whisper;
        }

        whisper = whisper.Substring(1);
        var commandArray = whisper.Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        var commands = commandArray.Aggregate((o, n) => $"{o}{n}").ToLowerInvariant();
        return (commands);
    }
}
