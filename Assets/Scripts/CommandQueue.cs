using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandQueue : MonoBehaviour
{
    //Queue's store and handle messege sending so the BOT does not break Twitch Dev Guidlines
    //100 chat messeges per min
    //200 whispers per min
    Queue<Arrrgs> commandQueueChat = new Queue<Arrrgs>();
    Queue<Arrrgs> commandQueueWhisper = new Queue<Arrrgs>();
    [SerializeField] Commands commands;
    
    private void Start()
    {
        StartCoroutine(RemoveFromChatQueue());
    }

    public void AddToChatQueue(Arrrgs arg)
    {
        commandQueueChat.Enqueue(arg);
        Debug.Log("Command in queue");
        Debug.Log("There are " + commandQueueChat.Count + " in the queue");
    }

    private void AddToWhisperQueue(Arrrgs arg)
    {
        commandQueueWhisper.Enqueue(arg);
    }

    //This Dequeue's from the commandChatQueue
    IEnumerator RemoveFromChatQueue()
    {
        while (true)
        {
            if (commandQueueChat.Count < 1) yield return new WaitForSeconds(0.3f);
            try {
                if (commandQueueChat.Count > 0) {
                    var e = commandQueueChat.Dequeue();
                    string firstCommand = e.commandText; //Command.CommandText.ToLower();
                    commands.ExecuteCommand(e);
                }
            } catch (Exception ex) {
                Debug.LogException(ex);
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    //This Seperates the different commands into buckets
    public void FirstCommandBuckets(Arrrgs e)
    {
        string firstCommand = e.commandText; //Command.CommandText.ToLower();
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
