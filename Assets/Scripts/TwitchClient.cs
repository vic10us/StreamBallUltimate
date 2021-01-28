#pragma warning disable 649

using System.Collections.Generic;
using UnityEngine;
using TwitchLib.Client.Models;
using TwitchLib.Unity;
using TwitchLib.Client.Events;
using System;
using UnityEngine.UI;

public class TwitchClient : MonoBehaviour
{
    // Client Object is defined in Twitch Lib
    public Client client;
    public JoinedChannel joinedChannel;
    public JoinedChannel botChannel;
    CommandQueue commandQueue;
    private PubSub pubSub;
    [SerializeField] Text debug;
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
        //Debug.Log($"{GlobalConfiguration.bot_name}: {GlobalConfiguration.bot_access_token}");
        bot_name = GlobalConfiguration.bot_name;
        channel_name = GlobalConfiguration.channel_name;
        ConnectionCredentials credentials = new ConnectionCredentials(GlobalConfiguration.bot_name, GlobalConfiguration.bot_access_token);

        client = new Client();
        client.Initialize(credentials, channel_name);
        //pubSub = new PubSub();

        //connect bot to channel
        client.Connect();
        client.OnJoinedChannel += ClientOnJoinedChannel;
        client.OnMessageReceived += MyMessageReceivedFunction;
        client.OnChatCommandReceived += MyCommandReceivedFunction;
        client.OnWhisperSent += Client_OnWhisperSent;
        client.OnWhisperReceived += Client_OnWhisperReceived;
        
        //pubSub.OnChannelCommerceReceived += Pubsub_OnCommerceReceived;

        //client.On will fill in with telesence
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
        //client.SendMessage(joinedChannel, "SimpaGameBotConnected");
    }

    private void MyCommandReceivedFunction(object sender, OnChatCommandReceivedArgs e)
    {
        Arrrgs chatArgs = new Arrrgs();
        chatArgs.MessageType = MessageType.Command;
        chatArgs.isMod = e.Command.ChatMessage.IsModerator;
        chatArgs.isBroadcaster = e.Command.ChatMessage.IsBroadcaster;
        chatArgs.message = e.Command.ChatMessage.Message;
        chatArgs.userID = e.Command.ChatMessage.UserId;
        chatArgs.displayName = e.Command.ChatMessage.DisplayName;
        chatArgs.commandText = e.Command.CommandText;
        chatArgs.multiCommand = e.Command.ArgumentsAsList;
        chatArgs.argumentsAsString = e.Command.ArgumentsAsString;

        for (int index = 0; index < e.Command.ArgumentsAsList.Count; index++)
        {
            chatArgs.commandArgs += e.Command.ArgumentsAsList[index].ToLower();
        }

        commandQueue.FirstCommandBuckets(chatArgs); //e
        debug.text = (e.Command.ChatMessage.Username);
    }

    private void MyMessageReceivedFunction(object sender,
        TwitchLib.Client.Events.OnMessageReceivedArgs e)
    {
        Debug.Log(e.ChatMessage.UserId);
    }

    private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
    {
        Arrrgs chatArgs = new Arrrgs();
        chatArgs.MessageType = MessageType.Whisper;
        chatArgs.message = e.WhisperMessage.Message;
        chatArgs.userID = e.WhisperMessage.UserId;
        chatArgs.displayName = e.WhisperMessage.DisplayName;
        chatArgs.commandText = ConvertWhisperToCommand(e.WhisperMessage.Message);
        chatArgs.commandArgs = ConvertWhisperToArguments(e.WhisperMessage.Message);
        commandQueue.FirstCommandBuckets(chatArgs);
    }

    private string ConvertWhisperToCommand(string whisper)
    {
        if (string.IsNullOrEmpty(whisper))
        {
            return whisper;
        }
        else
        {
            whisper = whisper.Substring(1);
            string[] commandArray = whisper.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return (commandArray[0].ToLower());
        }
    }
    
    private string ConvertWhisperToArguments(string whisper)
    {
        string commands = ""; 
        if (string.IsNullOrEmpty(whisper))
        {
            return whisper;
        }

        whisper = whisper.Substring(1);
        string[] commandArray = whisper.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        // Debug.Log(commandArray[0]);
        if (commandArray.Length > 1)
        {
            for (int i = 1; i < commandArray.Length; i++)
            {
                commands += commandArray[i];
            }
        }
        commands = commands.ToLower();
        return (commands);
    }

    private List<string> ParseCommand(string command)
    {
        List<string> list = new List<string>();
        string[] commandArray = command.Split(' ');
        if (string.IsNullOrEmpty(command))
        {
            return null;
        }
        // for (int i = 0; i < commandArray.Length; i++)
        // {
        //     Debug.Log(commandArray.Length + " : this is command Array Length");
        //     Debug.Log(commandArray[i] + " : this is command Array");
        // }
        return null;
    }
}
