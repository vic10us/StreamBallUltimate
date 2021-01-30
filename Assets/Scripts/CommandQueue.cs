#pragma warning disable 649
#pragma warning disable IDE0051 // Remove unused private members
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable IteratorNeverReturns

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandQueue : MonoBehaviour
{
    //Queue's store and handle message sending so the BOT does not break Twitch Dev Guidelines
    //100 chat messages per min
    //200 whispers per min
    private readonly Queue<Arrrgs> commandQueueChat = new Queue<Arrrgs>();
    private readonly Queue<Arrrgs> commandQueueWhisper = new Queue<Arrrgs>();
    [SerializeField] public Commands commands;
    
    private void Start()
    {
        StartCoroutine(RemoveFromChatQueue());
    }

    public void AddToChatQueue(Arrrgs arg)
    {
        commandQueueChat.Enqueue(arg);
    }

    private void AddToWhisperQueue(Arrrgs arg)
    {
        commandQueueWhisper.Enqueue(arg);
    }

    //This De-queue's from the commandChatQueue
    private IEnumerator RemoveFromChatQueue()
    {
        while (true)
        {
            if (commandQueueChat.Count < 1) yield return new WaitForSeconds(0.3f);
            try
            {
                if (commandQueueChat.Count > 0)
                {
                    var e = commandQueueChat.Dequeue();
                    commands.ExecuteCommand(e);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            yield return new WaitForSeconds(0.3f);
        }
    }

    //This Separates the different commands into buckets
    public void FirstCommandBuckets(Arrrgs e)
    {
        var firstCommand = e.commandText;
        //These commands will provide player confirmation/Response in CHAT
        var commandHandler = commands.GetCommandHandler(firstCommand);
        if (commandHandler == null) return;
        if (commandHandler.Queue) {
            AddToChatQueue(e);
        } else {
            commandHandler.Handle(e);
        }
    }

}
