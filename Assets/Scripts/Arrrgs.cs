using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrrgs
{
    //These are the chat arguments that we care about 
    //Stores data for both whispers and chat messeges 
    public string message = string.Empty;
    public string userID = string.Empty;
    public string displayName = string.Empty;
    public string commandText = string.Empty;
    public string commandArgs = string.Empty;
    public bool isMod = false;
    public bool isBroadcaster = false;
    public bool isAdmin => isMod || isBroadcaster;
    public List<string> multiCommand;

}
