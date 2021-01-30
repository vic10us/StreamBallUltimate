#pragma warning disable 649
#pragma warning disable IDE0051 // Remove unused private members
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable IteratorNeverReturns

using System.Collections.Generic;
using System.Linq;

public class Arrrgs
{
    //These are the chat arguments that we care about 
    //Stores data for both whispers and chat messages 
    public MessageType MessageType = MessageType.Command;
    public string message = string.Empty;
    public string userID = string.Empty;
    public string displayName = string.Empty;
    public string commandText = string.Empty;
    public string commandArgs = string.Empty;
    public string argumentsAsString;
    public List<string> multiCommand;
    public bool isMod = false;
    public bool isBroadcaster = false;
    public bool IsAdmin => isMod || isBroadcaster;
}
