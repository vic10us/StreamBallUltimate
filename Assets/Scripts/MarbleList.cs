using System.Collections;
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

    public List<bool> marbleArraySetToFalse = new List<bool>();
    //Every string needs to be the unique code for the marble
    //The bool will be true or false depending on if the player has unlocked that marble

    public Dictionary<string, int> marbleCommonNameToMarbleCode = new Dictionary<string, int>();
    //This is a way for the game to quickly look up what the unique marble code is for a common name

    public Dictionary<int, int> marbleCodeToCost = new Dictionary<int, int>();
    //This lets us look up the cost of a marble if we have the keycode

    public Dictionary<int, int> marbleCodeToRarity = new Dictionary<int, int>();
    //This links a marble code to its cost

    public Dictionary<int, string> marbleCodeToCommonName = new Dictionary<int, string>();

    [SerializeField] List<GameObject> marbles;
    //This game objects stores the balls uniquecode,skin(sprite),cost and rarity 

    private void LoadExternalMarbles(string path) {
        var currentMarbles = marbles.ToDictionary(m => m.name, m => m);
        var marbleImageFiles = Directory.GetFiles(path, "*.png");
        foreach (var marbleImageFile in marbleImageFiles) {
            var name = Path.GetFileNameWithoutExtension(marbleImageFile);
            if (currentMarbles.ContainsKey(name)) {
                Debug.Log($"{name} already exists in the marble list. Skipping");
                continue;
            }
            var code = currentMarbles.Count;
            var rarity = Random.Range(1, 5);
            Debug.Log($"Adding {name} to the marble list [Code: {code}, Rarity: {rarity}]");
            var newMarble = LoadNewMarble(code, name, rarity, marbleImageFile);
            marbles.Add(newMarble);
            currentMarbles.Add(name, newMarble);
        }
    }

    private void Start()
    {
        int code = 0;
        if (!marbles.Any()) return;
        if (MarblePrefab == null) return;

        LoadExternalMarbles(@"D:\Media\CustomMarbles");
        // var newMarble = LoadNewMarble(marbles.Count, "bun", 1, @"D:\Media\CustomMarbles\bun.png");
        // marbles.Add(newMarble);

        foreach (var item in marbles)
        {
            Debug.Log($"Iterating over marble : {item.GetInstanceID()}");
            //We grab the ball script at the start so we can build our dictionaries at start
            var marble = item.GetComponent<Marble>();
            marble.transform.position = new Vector3(0,0,0);
            marble.marbleCode = code;
            //We first need to set the code before we build the dictionaries 

            marbleArraySetToFalse.Add(false);
            marbleCommonNameToMarbleCode.Add((marble.commonName.ToLower()), marble.marbleCode);
            marbleCodeToCommonName.Add(marble.marbleCode, (marble.commonName.ToLower()));
            int cost = getMarbleCostsBasedOnRarity(marble.rarity);
            marbleCodeToCost.Add(marble.marbleCode, cost);
            marble.cost = cost;
            code++;
        }
        //The player always has marble 0 unlocked
        marbleArraySetToFalse[0] = true;
    }

    public List<bool> getEmptyAllMarbleDictionary() 
    {
        return marbleArraySetToFalse;
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

    public GameObject LoadNewMarble(int code, string commonName, int rarity, string imagePath) {
        var item = Instantiate(MarblePrefab);
        item.transform.position = new Vector3(0,0,0);
        Debug.Log($"Settting up new marble : {item.GetInstanceID()}");
        var marble = item.GetComponent<Marble>();
        marble.transform.position = new Vector3(0,0,0);
        marble.marbleCode = code;
        marble.commonName = commonName;
        marble.rarity = rarity;
        marble.marbleSprite = AddSprite(imagePath);
        marble.cost = getMarbleCostsBasedOnRarity(rarity);
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

    public int GetMarbleCodeFromCommonName(string commonName)
    {
        int code = marbleCommonNameToMarbleCode[commonName];
        return code;
    }

    public string GetCommonNameFromMarbleCode(int ballCode)
    {
        var go = marbles.FirstOrDefault(c => {
            var m = c.GetComponent<Marble>();
            return m.marbleCode.Equals(ballCode);
        });
        var marble = go?.GetComponent<Marble>();
        return marble?.commonName ?? "unknown";
        // return marbleCodeToCommonName[ballCode];
    }

    // public string GetCommonNameFromMarbleCode(int ballCode)
    // {
    //     return marbleCodeToCommonName[ballCode];
    // }

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
                return Random.Range(minCommon, maxCommon);
            case 2:
                return Random.Range(maxCommon, maxRare);
            case 3:
                return Random.Range(maxRare, maxEpic);
            case 4:
                return Random.Range(maxEpic, maxLegendary);
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
            int marbleCode = Random.Range(0, marbleLength);
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
