#pragma warning disable 649
#pragma warning disable IDE0051 // Remove unused private members
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable IteratorNeverReturns

using System.Collections.Generic;
using Newtonsoft.Json;

public class MarbleInfos {
    [JsonIgnore]
    public bool IsDirty { get; set; } = false;
    public List<MarbleInfo> Marbles { get; set; } = new List<MarbleInfo>();
}