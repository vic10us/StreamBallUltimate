using System.Collections.Generic;

public class PlayerData 
    //DATA that is stored in a dictionary and can be accessed by looking at the key which will be a player chat ID 
{
    public int money = 0;
    public List<string> purchasedSkins = new List<string>();
    public string playerName;
    public bool isSubscribed;
    public string selectedSkin;
    //Things I may want to add later bellow
    //public Dictionary<int, bool> sfx = new Dictionary<int, bool>();
    //public Dictionary<int, bool> vfx = new Dictionary<int, bool>();

}
