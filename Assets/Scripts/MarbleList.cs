using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class MarbleList : MonoBehaviour
{
    [SerializeField] int minCommon = 0;
    [SerializeField] int maxCommon = 100;
    [SerializeField] int maxRare = 400;
    [SerializeField] int maxEpic = 1000;
    [SerializeField] int maxLegendary = 2000;
    [SerializeField] GameObject MarblePrefab;
    // [SerializeField] string ExternalMarblesPath = @"D:\Media\CustomMarbles";
    [SerializeField] List<GameObject> marbles;

    private void LoadExternalMarbles() {
        var loadMarbles = GlobalConfiguration.GetValue("marbles", "LoadExternalMarbles").Equals("true", StringComparison.InvariantCultureIgnoreCase);
        var path = GlobalConfiguration.GetValue("marbles", "ExternalPath");
        if (!loadMarbles || string.IsNullOrWhiteSpace(path)) return;
        
        var marbleConfig = ConfigurationManager.GetConfig<MarbleInfos>(Path.Combine(path, "meta-data.json"), true);
        var marbleConfigMarbles = marbleConfig.Marbles.ToList();
        var currentMarbles = marbles.ToDictionary(m => m.name, m => m);
        var marbleImageFiles = Directory.GetFiles(path, "*.png");
        var marbleImageFileNames = marbleImageFiles.Select(mi => Path.GetFileName(mi));
        
        foreach (var info in marbleConfig.Marbles) {
            Debug.Log($"Found marble in config. Loading... {info.FileName}");
            var fileName = Path.Combine(path, info.FileName);
            if (!File.Exists(fileName)) continue;
            var newMarble = LoadNewMarble(info.CommonName, info.Rarity, fileName, info.Cost);
            marbles.Add(newMarble);
            currentMarbles.Add(info.CommonName, newMarble);
        }

        foreach (var marbleImageFile in 
                     marbleImageFileNames
                        .Where(min => !marbleConfig.Marbles.Any(fn => fn.FileName.Equals(min, StringComparison.InvariantCultureIgnoreCase)))) 
        {
            var fullFileName = Path.Combine(path, marbleImageFile);
            var name = Path.GetFileNameWithoutExtension(marbleImageFile);
            if (currentMarbles.ContainsKey(name)) {
                Debug.Log($"{name} already exists in the marble list. Skipping");
                continue;
            }
            var rarity = UnityEngine.Random.Range(1, 5);
            Debug.Log($"Adding {name} to the marble list [Rarity: {rarity}]");
            var newMarble = LoadNewMarble(name, rarity, fullFileName);
            var cost = newMarble.GetComponent<Marble>().cost;
            marbles.Add(newMarble);
            currentMarbles.Add(name, newMarble);
            marbleConfigMarbles.Add(new MarbleInfo {
                CommonName = name,
                Cost = cost,
                FileName = marbleImageFile,
                Rarity = rarity
            });
            marbleConfig.IsDirty = true;
        }

        if (marbleConfig.IsDirty) {
            marbleConfig.Marbles = marbleConfigMarbles;
            // Debug.LogWarning($"{Path.Combine(path, "meta-data.json")}");
            ConfigurationManager.SaveConfig(Path.Combine(path, "meta-data.json"), marbleConfig);
        }
    }

    private static System.Random random = new System.Random();
    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
        .Select(s => s[random.Next(s.Length)]).ToArray()).ToLowerInvariant();
    }

    private void Start()
    {
        if (!marbles.Any()) return;
        if (MarblePrefab == null) return;

        LoadExternalMarbles();

        while (marbles.Count < 3) {
            var newMarble = CreateDummyMarble(RandomString(8));
            marbles.Add(newMarble);
        }

        foreach (var item in marbles)
        {
            Debug.Log($"Iterating over marble : {item.GetInstanceID()}");
            var marble = item.GetComponent<Marble>();
            marble.transform.position = new Vector3(0,0,0);
            if (marble.cost < 0) {
                int cost = getMarbleCostsBasedOnRarity(marble.rarity);
                marble.cost = cost;
            }
        }
    }

    public static Texture2D Resize(Texture2D texture2D,int targetX,int targetY)
    {
        RenderTexture rt=new RenderTexture(targetX, targetY,24);
        RenderTexture.active = rt;
        Graphics.Blit(texture2D,rt);
        Texture2D result=new Texture2D(targetX,targetY);
        result.ReadPixels(new Rect(0,0,targetX,targetY),0,0);
        result.Apply();
        return result;
    }

    public static Texture2D LoadMarblePNG(string filePath) {
    
        Texture2D tex = null;
        byte[] fileData;
    
        if (File.Exists(filePath)) {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(512, 512);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        tex = Resize(tex, 512, 512);
        return tex;
    }
    
    public Sprite AddSprite(string filePath) {
        Texture2D _texture = LoadMarblePNG(filePath); // Resources.Load<Texture2D>("texture2") as Texture2D;
        Sprite newSprite = Sprite.Create(_texture, new Rect(0f, 0f, _texture.width, _texture.height), new Vector2(0.5f, 0.5f), 100f);
        newSprite.name = Path.GetFileNameWithoutExtension(filePath);
        return newSprite;
    }

    public GameObject CreateDummyMarble(string commonName) {
        var item = Instantiate(MarblePrefab);
        item.transform.position = new Vector3(0,0,0);
        Debug.Log($"Settting up new DUMMY marble : {item.GetInstanceID()}");
        var marble = item.GetComponent<Marble>();
        marble.transform.position = new Vector3(0,0,0);
        marble.commonName = commonName;
        marble.rarity = 4;
        marble.marbleSprite = Resources.Load<Sprite>("Sprites/unknown");
        marble.cost = 99999;
        marble.name = commonName;
        return item;
    }

    public GameObject LoadNewMarble(string commonName, int rarity, string imagePath, int cost = -1) {
        var item = Instantiate(MarblePrefab);
        item.transform.position = new Vector3(0,0,0);
        Debug.Log($"Settting up new marble : {item.GetInstanceID()}");
        var marble = item.GetComponent<Marble>();
        marble.transform.position = new Vector3(0,0,0);
        marble.commonName = commonName;
        marble.rarity = rarity;
        marble.marbleSprite = AddSprite(imagePath);
        marble.cost = cost >= 0 ? cost : getMarbleCostsBasedOnRarity(rarity);
        marble.name = commonName;
        return item;
    }

    public bool DoesMarbleCommonNameExist(string commonName)
    {
        return GetMarbleGameObject(commonName) != null;
    }

    public int GetMarbleCostFromCommonName(string commonName)
    {
        var marble = GetMarble(commonName);
        return marble.cost;
    }

    public GameObject GetMarbleFromMarbleCode(int ballCode)
    {
        return marbles[ballCode];
    }
    
    public int getMarbleCostsBasedOnRarity(int rarity)
    {
        //At the start of the game session all the marble costs will be randomized 
        //They will fall into a price range depending on rarity
        switch (rarity)
        {
            case 1:
                return UnityEngine.Random.Range(minCommon, maxCommon);
            case 2:
                return UnityEngine.Random.Range(maxCommon, maxRare);
            case 3:
                return UnityEngine.Random.Range(maxRare, maxEpic);
            case 4:
                return UnityEngine.Random.Range(maxEpic, maxLegendary);
            default:
                return -1;
        }
    }

    public HashSet<GameObject> GetMarblesForShop(int howManyMarbles)
    {
        int marbleLength = marbles.Count;
        HashSet<GameObject> returnedMarbles = new HashSet<GameObject>();
        if (!marbles.Any()) {
            Debug.LogError("There are NO marbles to load into the shop. :(");
            return returnedMarbles;
        }
        do { 
            int marbleCode = UnityEngine.Random.Range(0, marbleLength);
            returnedMarbles.Add(marbles[marbleCode]);
        } while (returnedMarbles.Count<howManyMarbles);
        foreach (var item in returnedMarbles)
        {
            Debug.Log(item.GetComponent<Marble>().commonName);
        }
        return returnedMarbles;
    }

    public Marble GetMarble(string commonName)
    {
        return GetMarbleGameObject(commonName).GetComponent<Marble>();
    }

    public GameObject GetMarbleGameObject(string commonName) {
        Debug.Log($"Looking for marble with the name of '{commonName}'");
        return marbles.FirstOrDefault(c => {
            var m = c.GetComponent<Marble>();
            return m.commonName.Equals(commonName, System.StringComparison.InvariantCultureIgnoreCase);
        });
    }
}
