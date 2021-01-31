#pragma warning disable 649
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable IteratorNeverReturns

using System.Collections.Generic;

public class Arrrgs
{
    //These are the chat arguments that we care about 
    //Stores data for both whispers and chat messages 
    public MessageType MessageType { get; set; } = MessageType.Command;
    public string Message { get; set; } = string.Empty;
    public string UserID { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string CommandText { get; set; } = string.Empty;
    public string CommandArgs { get; set; } = string.Empty;
    public string ArgumentsAsString { get; set; }
    public List<string> MultiCommand { get; set; } = new List<string>();
    public bool IsMod { get; set; } = false;
    public bool IsBroadcaster { get; set; } = false;

    public bool IsAdmin => IsMod || IsBroadcaster;
}
