#pragma warning disable 649
#pragma warning disable IDE0051 // Remove unused private members
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable IteratorNeverReturns

using System.Collections.Generic;
using Newtonsoft.Json;

//DATA that is stored in a dictionary and can be accessed by looking at the key which will be a player chat ID 
public class PlayerData
{
    [JsonProperty("money")]
    public int Money { get; set; } = 0;
    [JsonProperty("purchasedSkins")]
    public List<string> PurchasedSkins { get; set; } = new List<string>();
    [JsonProperty("playerName")]
    public string PlayerName { get; set; }
    [JsonProperty("isSubscribed")]
    public bool IsSubscribed { get; set; }
    [JsonProperty("selectedSkin")]
    public string SelectedSkin { get; set; }
    //Things I may want to add later bellow
    //public Dictionary<int, bool> sfx = new Dictionary<int, bool>();
    //public Dictionary<int, bool> vfx = new Dictionary<int, bool>();
}
