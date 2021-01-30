#pragma warning disable 649
#pragma warning disable IDE0051 // Remove unused private members
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable IteratorNeverReturns

using System;

public class CommandHandler {
    public bool Queue { get; set; } = true;
    public Action<Arrrgs> Handle;
}

public class HelpInfo
{

}

public class HelpInfoAttribute : Attribute
{
    public string HelpText { get; set; } = "There is no help for this command";
}

public class CommandAttribute : Attribute
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string MatchExpression { get; set; }
    public bool Queue { get; set; } = true;
    public bool AdminOnly { get; set; } = false;
    public bool Enabled { get; set; } = true;
}