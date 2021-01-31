#pragma warning disable 649
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable IteratorNeverReturns

using System;

public class CommandAttribute : Attribute
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string MatchExpression { get; set; }
    public bool Queue { get; set; } = true;
    public bool AdminOnly { get; set; } = false;
    public bool Enabled { get; set; } = true;
}