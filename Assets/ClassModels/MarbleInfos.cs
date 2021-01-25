using System.Collections.Generic;
using Newtonsoft.Json;

public class MarbleInfos {
    [JsonIgnore]
    public bool IsDirty { get; set; } = false;
    public List<MarbleInfo> Marbles { get; set; } = new List<MarbleInfo>();
}