#pragma warning disable 649
#pragma warning disable IDE0051 // Remove unused private members
// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable IteratorNeverReturns

using UnityEngine;

public class Marble : MonoBehaviour
{
    public string commonName; //name for player to see and call, not necessarily unique
    public int cost;
    public int rarity; //use scale 1-4 inclusive, used to determine cost and chance to appear in the shop
    public Sprite marbleSprite;

    void Start()
    {
        // marbleSprite = GetComponent<SpriteRenderer>().sprite;
    }

    void Update()
    {
        
    }
}
